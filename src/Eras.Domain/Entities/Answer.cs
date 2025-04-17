using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Answer : BaseEntity, IAuditableEntity
    {
        public string AnswerText { get; set; } = string.Empty;
        public int RiskLevel { get; set; }
        public Variable Variable { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
        public int PollInstanceId { get; set; }
        public PollInstance PollInstance { get; set; } = default!;
        public int PollVariableId { get; set; }
    }
}
