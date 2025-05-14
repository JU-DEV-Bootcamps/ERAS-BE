using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Answer : BaseEntity, IAuditableEntity, IVersionableEntity
    {
        public string AnswerText { get; set; } = string.Empty;
        public int RiskLevel { get; set; }
        public Variable Variable { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
        public VersionInfo Version { get; set; } = default!;
        public int PollInstanceId { get; set; }
        public int PollVariableId { get; set; }
    }
}
