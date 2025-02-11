using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class PollEntity : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public ICollection<PollVariableMapping> PollVariables { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}
