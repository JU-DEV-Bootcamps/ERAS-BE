using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Poll : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public AuditInfo Audit { get; set; } = default!;
        public ICollection<Variable> Variables { get; set; } = [];
    }
}
