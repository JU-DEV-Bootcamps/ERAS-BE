using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class ImportJobEntity : BaseEntity
    {
        public int EvaluationId { get; set; }
        public ImportJobStatus Status { get; set; } = ImportJobStatus.Queued;
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public string PollsPayload { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
