using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    /// <summary>
    /// Per-student unit of work within an <see cref="ImportJob"/>. Each item corresponds to one
    /// incoming PollDTO (one student's submission) and is processed/retried independently.
    /// </summary>
    public class ImportJobItem : BaseEntity
    {
        public int ImportJobId { get; set; }
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? Cohort { get; set; }
        public ImportJobStatus Status { get; set; } = ImportJobStatus.Queued;
        public int RetryCount { get; set; }
        public bool IsAlreadyImported { get; set; }
        public string? ErrorMessage { get; set; }

        /// <summary>JSON-serialized PollDTO for this single student, used for independent retry.</summary>
        public string PollPayload { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
