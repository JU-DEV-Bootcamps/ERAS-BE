using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class ComponentEntity : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<VariableEntity> Variables { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}