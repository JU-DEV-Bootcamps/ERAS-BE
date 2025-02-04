using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Cohort: BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = default!;
        public string CourseCode { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
        public ICollection<Student> Students { get; set; } = [];
  }
}
