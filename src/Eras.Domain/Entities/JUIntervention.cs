using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class JUIntervention: BaseEntity, IAuditableEntity
    {
        public string Diagnostic { get; set; } = string.Empty;
        public string Objective { get; set; } = string.Empty;
        public int StudentId { get; set; } = default!;
        public IEnumerable<JURemission> Remissions { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}