using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class ImportJobItemEntity : BaseEntity
    {
        public int ImportJobId { get; set; }
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? Cohort { get; set; }
        public ImportJobStatus Status { get; set; } = ImportJobStatus.Queued;
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public string PollPayload { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
