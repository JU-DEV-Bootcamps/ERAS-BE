using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Queries.GetConfiguration;
using Eras.Application.Services;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eras.Infrastructure.BackgroundProcessing
{
    /// <summary>
    /// Consumes queued import job ids and processes them in their own DI scope (same pattern as
    /// <c>EvaluationStatusSyncJob</c>). A job has two phases driven by its status:
    /// <list type="bullet">
    /// <item><b>Extracting</b>: fetch respondents from Cosmic Latte, creating one
    /// <see cref="ImportJobItem"/> per student (Extracted) with its payload; then → Ready.</item>
    /// <item><b>Importing</b> (items in Queued): resolve the poll structure once, then persist each
    /// confirmed student in its own transaction so one failure doesn't roll back the rest.</item>
    /// </list>
    /// A failure never stops the host or the queue loop.
    /// </summary>
    public sealed class ImportQueueBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IImportJobQueue _queue;
        private readonly ILogger<ImportQueueBackgroundService> _logger;

        public ImportQueueBackgroundService(
            IServiceScopeFactory ScopeFactory,
            IImportJobQueue Queue,
            ILogger<ImportQueueBackgroundService> Logger)
        {
            _scopeFactory = ScopeFactory;
            _queue = Queue;
            _logger = Logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                int importJobId;
                try
                {
                    importJobId = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    await ProcessAsync(importJobId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error processing import job {Id}", importJobId);
                    await TryMarkJobFailedAsync(importJobId, ex.Message);
                }
            }
        }

        private async Task ProcessAsync(int importJobId)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var jobRepository = scope.ServiceProvider.GetRequiredService<IImportJobRepository>();

            ImportJob? job = await jobRepository.GetByIdAsync(importJobId);
            if (job == null)
            {
                _logger.LogWarning("Import job {Id} not found; skipping", importJobId);
                return;
            }

            if (job.Status == ImportJobStatus.Extracting)
            {
                await ExtractAsync(scope, job);
            }
            else
            {
                await ImportAsync(scope, job);
            }
        }

        private async Task ExtractAsync(IServiceScope scope, ImportJob job)
        {
            var jobRepository = scope.ServiceProvider.GetRequiredService<IImportJobRepository>();
            var itemRepository = scope.ServiceProvider.GetRequiredService<IImportJobItemRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var cosmicLatte = scope.ServiceProvider.GetRequiredService<ICosmicLatteAPIService>();

            var configuration = await mediator.Send(new GetConfigurationQuery { ConfigurationId = job.ConfigurationId });

            int extracted = 0;
            await cosmicLatte.ExtractRespondentsAsync(
                job.EvaluationSetName ?? string.Empty,
                job.StartDate ?? string.Empty,
                job.EndDate ?? string.Empty,
                configuration.EncryptedKey,
                configuration.BaseURL,
                async (poll, alreadyImported) =>
                {
                    StudentDTO? student = poll.Components?.FirstOrDefault()?.Variables?.FirstOrDefault()?.Answer?.Student;
                    DateTime now = DateTime.UtcNow;
                    await itemRepository.AddAsync(new ImportJobItem
                    {
                        ImportJobId = job.Id,
                        StudentEmail = student?.Email ?? string.Empty,
                        StudentName = student?.Name ?? string.Empty,
                        Cohort = student?.Cohort?.Name,
                        Status = ImportJobStatus.Extracted,
                        IsAlreadyImported = alreadyImported,
                        PollPayload = JsonSerializer.Serialize(poll),
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now,
                    });
                    extracted++;
                    await jobRepository.SetExtractedCountAsync(job.Id, extracted, now);
                });

            await jobRepository.SetReadyAsync(job.Id, extracted, DateTime.UtcNow);
        }

        private async Task ImportAsync(IServiceScope scope, ImportJob job)
        {
            var jobRepository = scope.ServiceProvider.GetRequiredService<IImportJobRepository>();
            var itemRepository = scope.ServiceProvider.GetRequiredService<IImportJobItemRepository>();
            var orchestrator = scope.ServiceProvider.GetRequiredService<PollOrchestratorService>();

            List<ImportJobItem> queuedItems = await itemRepository.GetByJobIdAndStatusAsync(job.Id, ImportJobStatus.Queued);
            if (queuedItems.Count == 0)
            {
                await UpdateAggregateStatusAsync(jobRepository, itemRepository, job.Id);
                return;
            }

            await jobRepository.SetStatusAsync(job.Id, ImportJobStatus.Importing, DateTime.UtcNow);

            // Resolve/create the poll template once (all confirmed students share it). The structure
            // is derived from the first item's payload so it works for the extract flow too.
            PollDTO firstPoll = JsonSerializer.Deserialize<PollDTO>(queuedItems[0].PollPayload)!;
            var setup = await orchestrator.SetupImportStructureAsync([firstPoll], job.EvaluationId);
            if (!setup.Success)
            {
                await jobRepository.SetResultAsync(job.Id, ImportJobStatus.Failed, 0, setup.Message, DateTime.UtcNow);
                return;
            }

            foreach (ImportJobItem item in queuedItems)
            {
                await itemRepository.SetStatusAsync(item.Id, ImportJobStatus.Running, null, DateTime.UtcNow);

                PollDTO poll = JsonSerializer.Deserialize<PollDTO>(item.PollPayload)!;
                ImportStudentResult result = await orchestrator.ProcessStudentAsync(poll, job.EvaluationId);

                ImportJobStatus itemStatus = result.Success ? ImportJobStatus.Completed : ImportJobStatus.Failed;
                await itemRepository.SetStatusAsync(item.Id, itemStatus, result.ErrorMessage, DateTime.UtcNow);
            }

            await UpdateAggregateStatusAsync(jobRepository, itemRepository, job.Id);
        }

        private static async Task UpdateAggregateStatusAsync(
            IImportJobRepository jobRepository,
            IImportJobItemRepository itemRepository,
            int jobId)
        {
            (int pending, int completed, int failed) = await itemRepository.GetImportPhaseCountsAsync(jobId);

            ImportJobStatus status = pending > 0
                ? ImportJobStatus.Importing
                : failed == 0
                    ? ImportJobStatus.Completed
                    : completed > 0 ? ImportJobStatus.PartiallyCompleted : ImportJobStatus.Failed;

            await jobRepository.SetResultAsync(jobId, status, completed, null, DateTime.UtcNow);
        }

        private async Task TryMarkJobFailedAsync(int importJobId, string message)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var jobRepository = scope.ServiceProvider.GetRequiredService<IImportJobRepository>();
                await jobRepository.SetResultAsync(importJobId, ImportJobStatus.Failed, 0, message, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark import job {Id} as Failed", importJobId);
            }
        }
    }
}
