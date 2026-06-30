using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Services;
using Eras.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eras.Infrastructure.BackgroundProcessing
{
    /// <summary>
    /// Consumes queued import job ids and processes them in their own DI scope (same pattern as
    /// <c>EvaluationStatusSyncJob</c>). Each job: resolves the poll structure once, then processes
    /// every <see cref="ImportJobStatus.Queued"/> item (one student) in its own transaction, so a
    /// single student's failure does not roll back the others. Failed items are surfaced for manual
    /// retry via the API rather than retried automatically.
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

                await ProcessAsync(importJobId);
            }
        }

        private async Task ProcessAsync(int importJobId)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var jobRepository = scope.ServiceProvider.GetRequiredService<IImportJobRepository>();
            var itemRepository = scope.ServiceProvider.GetRequiredService<IImportJobItemRepository>();
            var orchestrator = scope.ServiceProvider.GetRequiredService<PollOrchestratorService>();

            ImportJob? job = await jobRepository.GetByIdAsync(importJobId);
            if (job == null)
            {
                _logger.LogWarning("Import job {Id} not found; skipping", importJobId);
                return;
            }

            List<ImportJobItem> queuedItems = await itemRepository.GetByJobIdAndStatusAsync(importJobId, ImportJobStatus.Queued);
            if (queuedItems.Count == 0)
            {
                await UpdateAggregateStatusAsync(jobRepository, itemRepository, job);
                return;
            }

            job.Status = ImportJobStatus.Running;
            job.UpdatedAtUtc = DateTime.UtcNow;
            await jobRepository.UpdateAsync(job);

            // Resolve/create the poll template once for the whole job (idempotent on retries).
            List<PollDTO> polls = JsonSerializer.Deserialize<List<PollDTO>>(job.PollsPayload) ?? [];
            var setup = await orchestrator.SetupImportStructureAsync(polls, job.EvaluationId);
            if (!setup.Success)
            {
                job.Status = ImportJobStatus.Failed;
                job.ErrorMessage = setup.Message;
                job.UpdatedAtUtc = DateTime.UtcNow;
                await jobRepository.UpdateAsync(job);
                return;
            }

            foreach (ImportJobItem item in queuedItems)
            {
                item.Status = ImportJobStatus.Running;
                item.UpdatedAtUtc = DateTime.UtcNow;
                await itemRepository.UpdateAsync(item);

                PollDTO poll = JsonSerializer.Deserialize<PollDTO>(item.PollPayload)!;
                ImportStudentResult result = await orchestrator.ProcessStudentAsync(poll, job.EvaluationId);

                item.Status = result.Success ? ImportJobStatus.Completed : ImportJobStatus.Failed;
                item.ErrorMessage = result.ErrorMessage;
                item.UpdatedAtUtc = DateTime.UtcNow;
                await itemRepository.UpdateAsync(item);
            }

            await UpdateAggregateStatusAsync(jobRepository, itemRepository, job);
        }

        private static async Task UpdateAggregateStatusAsync(
            IImportJobRepository jobRepository,
            IImportJobItemRepository itemRepository,
            ImportJob job)
        {
            List<ImportJobItem> allItems = await itemRepository.GetByJobIdAsync(job.Id);
            int completed = allItems.Count(I => I.Status == ImportJobStatus.Completed);
            int failed = allItems.Count(I => I.Status == ImportJobStatus.Failed);

            job.ProcessedCount = completed;
            job.Status = failed == 0
                ? ImportJobStatus.Completed
                : completed > 0 ? ImportJobStatus.PartiallyCompleted : ImportJobStatus.Failed;
            job.UpdatedAtUtc = DateTime.UtcNow;
            await jobRepository.UpdateAsync(job);
        }
    }
}
