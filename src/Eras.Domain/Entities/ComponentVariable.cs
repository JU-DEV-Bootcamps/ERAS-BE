using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class ComponentVariable : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public int PollId { get; set; }
        public virtual Poll? Poll { get; set; }
        public int Position { get; set; }
        public int? ParentId { get; set; }
        public virtual ComponentVariable Parent { get; set; } = default!;        
        
        public AuditInfo Audit { get; set; } = default!;
    }
}
