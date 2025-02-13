using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class VariableEntity : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public int ComponentId { get; set; }
        public ComponentEntity Component { get; set; } = default!;
        public ICollection<PollVariableJoin> PollVariables { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}