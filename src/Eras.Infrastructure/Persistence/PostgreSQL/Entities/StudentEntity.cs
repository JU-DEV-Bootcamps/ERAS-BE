using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class StudentEntity : BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool IsImported { get; set; } = false;
        public StudentDetailEntity? StudentDetail { get; set; }
        public ICollection<PollInstanceEntity> PollInstances { get; set; } = [];
        public ICollection<CohortEntity> Cohorts { get; set; } = [];
        public ICollection<StudentCohortJoin> StudentCohorts { get; set; } = [];
        public ICollection<int> RemissionIds { get; set; } = [];
        public ICollection<JURemissionEntity> Remissions { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}