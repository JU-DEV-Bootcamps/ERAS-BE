using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Answer : BaseEntity, IAuditableEntity
    {
        public string AnswerText { get; set; } = string.Empty;
        public int RiskLevel { get; set; }
        public Variable? Variable { get; set; }
        public AuditInfo Audit { get; set; } = default!;
    }
}
