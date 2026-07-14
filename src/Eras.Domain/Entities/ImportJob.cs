using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public enum ImportJobStatus
    {
        // Import-phase (and item) statuses
        Queued,
        Running,
        Completed,
        Failed,
        PartiallyCompleted,
        // Extraction-phase statuses
        Extracting,
        Extracted,
        Ready,
        Importing,
        Skipped
    }

    /// <summary>
    /// Tracks an asynchronous Cosmic Latte import: first the answers are extracted from Cosmic Latte
    /// in the background (Extracting → Ready), then the user confirms which respondents to persist
    /// (Importing → Completed). Survives restarts/retries since payloads live on the items.
    /// </summary>
    public class ImportJob : BaseEntity
    {
        public int EvaluationId { get; set; }
        public ImportJobStatus Status { get; set; } = ImportJobStatus.Queued;
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int ExtractedCount { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }

        // Extraction parameters (used by the background extraction phase).
        public string? EvaluationSetName { get; set; }
        public int ConfigurationId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        /// <summary>JSON-serialized list of PollDTO (legacy direct-import path); empty for the extract flow.</summary>
        public string PollsPayload { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
