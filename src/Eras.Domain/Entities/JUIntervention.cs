using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class JUIntervention: BaseEntity, IAuditableEntity
    {
        public string Diagnostic { get; set; } = string.Empty;
        public string Objective { get; set; } = string.Empty;
        public int StudentId { get; set; } = default!;
        public Student? Student { get; set; }
        public IEnumerable<int> RemissionIds { get; set; } = [];
        public List<JURemission> Remissions { get; set; } = [];
        public AuditInfo Audit { get; set; } = new AuditInfo()
        {
            CreatedBy = "Default constructor",
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
        };
    }
}