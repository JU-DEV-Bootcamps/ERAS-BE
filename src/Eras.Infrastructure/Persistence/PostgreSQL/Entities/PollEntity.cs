using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class PollEntity : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public ICollection<PollVariableJoin> PollVariables { get; set; } = [];
        public ICollection<EvaluationPollJoin> EvaluationPolls { get; set; } = [];
        public int LastVersion { get; set; }
        public DateTime LastVersionDate { get; set; }
        public AuditInfo Audit { get; set; } = default!;
        public string ParentId { get; set; } = string.Empty;
    }
}
