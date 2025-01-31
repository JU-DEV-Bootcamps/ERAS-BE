using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Component : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Variable> Variables { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}