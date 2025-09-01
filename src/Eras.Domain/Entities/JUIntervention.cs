using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class JUIntervention: BaseEntity, IAuditableEntity
    {
        public string Diagnostic { get; set; } = string.Empty;
        public string Objective { get; set; } = string.Empty;
        public Student Student { get; set; } = default!;
        public IEnumerable<JURemission> Remissions { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}