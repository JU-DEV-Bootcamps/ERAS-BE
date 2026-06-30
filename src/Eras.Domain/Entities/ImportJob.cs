using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public enum ImportJobStatus
    {
        Queued,
        Running,
        Completed,
        Failed,
        PartiallyCompleted
    }

    /// <summary>
    /// Tracks an asynchronous Cosmic Latte import so it can be processed in the background and
    /// polled by the client. The poll payload is persisted so the job survives restarts and retries.
    /// </summary>
    public class ImportJob : BaseEntity
    {
        public int EvaluationId { get; set; }
        public ImportJobStatus Status { get; set; } = ImportJobStatus.Queued;
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }

        /// <summary>JSON-serialized list of PollDTO to import.</summary>
        public string PollsPayload { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
