using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    /// <summary>
    /// Creates and queues asynchronous import jobs (with one item per student), exposes their status
    /// for polling, and re-queues selected failed items for retry.
    /// </summary>
    public class ImportJobService : IImportJobService
    {
        private const int MaxPollNameLength = 100;

        private readonly IImportJobRepository _importJobRepository;
        private readonly IImportJobItemRepository _importJobItemRepository;
        private readonly IImportJobQueue _queue;

        public ImportJobService(
            IImportJobRepository ImportJobRepository,
            IImportJobItemRepository ImportJobItemRepository,
            IImportJobQueue Queue)
        {
            _importJobRepository = ImportJobRepository;
            _importJobItemRepository = ImportJobItemRepository;
            _queue = Queue;
        }

        public async Task<int> QueueImportAsync(List<PollDTO> Polls, int EvaluationId)
        {
            if (Polls.Any(P => P.Name?.Length > MaxPollNameLength))
            {
                throw new ArgumentException("There was an error during the import: Poll Name exceeds the maximum length of 100 characters.");
            }

            DateTime now = DateTime.UtcNow;
            ImportJob job = new ImportJob
            {
                EvaluationId = EvaluationId,
                Status = ImportJobStatus.Queued,
                TotalCount = Polls.Count,
                PollsPayload = JsonSerializer.Serialize(Polls),
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
            };
            ImportJob created = await _importJobRepository.AddAsync(job);

            // One item per student so progress and retries are tracked individually.
            foreach (PollDTO poll in Polls)
            {
                StudentDTO? student = poll.Components?.FirstOrDefault()?.Variables?.FirstOrDefault()?.Answer?.Student;
                ImportJobItem item = new ImportJobItem
                {
                    ImportJobId = created.Id,
                    StudentEmail = student?.Email ?? string.Empty,
                    StudentName = student?.Name ?? string.Empty,
                    Cohort = student?.Cohort?.Name,
                    Status = ImportJobStatus.Queued,
                    PollPayload = JsonSerializer.Serialize(poll),
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now,
                };
                await _importJobItemRepository.AddAsync(item);
            }

            await _queue.EnqueueAsync(created.Id);
            return created.Id;
        }

        public async Task<ImportJobStatusDTO?> GetStatusAsync(int ImportJobId)
        {
            ImportJob? job = await _importJobRepository.GetByIdAsync(ImportJobId);
            if (job == null) return null;

            return new ImportJobStatusDTO
            {
                ImportJobId = job.Id,
                EvaluationId = job.EvaluationId,
                Status = job.Status.ToString(),
                TotalCount = job.TotalCount,
                ProcessedCount = job.ProcessedCount,
                RetryCount = job.RetryCount,
                ErrorMessage = job.ErrorMessage,
                CreatedAtUtc = job.CreatedAtUtc,
                UpdatedAtUtc = job.UpdatedAtUtc,
            };
        }

        public async Task<List<ImportJobItemDTO>> GetItemsAsync(int ImportJobId)
        {
            List<ImportJobItem> items = await _importJobItemRepository.GetByJobIdAsync(ImportJobId);
            return items.Select(Item => new ImportJobItemDTO
            {
                Id = Item.Id,
                ImportJobId = Item.ImportJobId,
                StudentEmail = Item.StudentEmail,
                StudentName = Item.StudentName,
                Cohort = Item.Cohort,
                Status = Item.Status.ToString(),
                RetryCount = Item.RetryCount,
                ErrorMessage = Item.ErrorMessage,
            }).ToList();
        }

        /// <summary>
        /// Re-queues the given failed items for processing and re-enqueues the job so the worker
        /// reprocesses them. Returns false if the job does not exist.
        /// </summary>
        public async Task<bool> RetryItemsAsync(int ImportJobId, List<int> ItemIds)
        {
            ImportJob? job = await _importJobRepository.GetByIdAsync(ImportJobId);
            if (job == null) return false;

            List<ImportJobItem> items = await _importJobItemRepository.GetByIdsAsync(ImportJobId, ItemIds);
            List<ImportJobItem> failed = items.Where(Item => Item.Status == ImportJobStatus.Failed).ToList();
            if (failed.Count == 0) return true;

            DateTime now = DateTime.UtcNow;
            foreach (ImportJobItem item in failed)
            {
                item.Status = ImportJobStatus.Queued;
                item.RetryCount += 1;
                item.ErrorMessage = null;
                item.UpdatedAtUtc = now;
                await _importJobItemRepository.UpdateAsync(item);
            }

            job.Status = ImportJobStatus.Queued;
            job.UpdatedAtUtc = now;
            await _importJobRepository.UpdateAsync(job);

            await _queue.EnqueueAsync(ImportJobId);
            return true;
        }
    }
}
