using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class CohortEntity: BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = default!;
        public string CourseCode { get; set; } = default!;
        public ICollection<StudentEntity> Students { get; set; } = [];
        public ICollection<StudentCohortJoin> StudentCohorts { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
  }
}
