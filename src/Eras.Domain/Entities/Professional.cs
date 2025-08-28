using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Professional: BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public AuditInfo Audit { get; set; } = default!;
    }
}