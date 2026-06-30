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

                // A failure while processing one job must never stop the host or the queue loop.
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
                await UpdateAggregateStatusAsync(jobRepository, itemRepository, job.Id);
                return;
            }

            await jobRepository.SetStatusAsync(job.Id, ImportJobStatus.Running, DateTime.UtcNow);

            // Resolve/create the poll template once for the whole job (idempotent on retries).
            List<PollDTO> polls = JsonSerializer.Deserialize<List<PollDTO>>(job.PollsPayload) ?? [];
            var setup = await orchestrator.SetupImportStructureAsync(polls, job.EvaluationId);
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
            (int completed, int failed, int total) = await itemRepository.GetStatusCountsAsync(jobId);

            // Don't declare a job terminal while items are still pending (covers a worker that
            // dequeued before all items were committed).
            bool hasPending = completed + failed < total;
            ImportJobStatus status = hasPending
                ? ImportJobStatus.Running
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
