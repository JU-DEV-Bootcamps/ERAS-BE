using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Variable : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int ComponentId { get; set; }
        public Component Component { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
        public ICollection<Poll> Polls { get; set; } = [];

    }
}