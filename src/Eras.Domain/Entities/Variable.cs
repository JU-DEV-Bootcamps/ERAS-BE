using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Variable : BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public AuditInfo Audit { get; set; } = default!;
        public int IdComponent { get; set; }
        public int PollVariableId { get; set; }
        public int IdPoll { get; set; }
    }
}
