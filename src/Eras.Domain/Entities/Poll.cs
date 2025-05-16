using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Poll : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public AuditInfo Audit { get; set; } = default!;
        public int LastVersion { get; set; }
        public DateTime LastVersionDate { get; set; }
        public ICollection<Component> Components { get; set; } = [];
    }
}
