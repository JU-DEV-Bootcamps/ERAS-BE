using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class AnswerEntity : BaseEntity, IAuditableEntity
    {
        public string AnswerText { get; set; } = string.Empty;
        public int RiskLevel { get; set; }
        public int PollInstanceId { get; set; }
        public PollInstanceEntity PollInstance { get; set; } = default!;
        public int PollVariableId { get; set; } = default!;
        public PollVariableJoin PollVariable { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
    }
}
