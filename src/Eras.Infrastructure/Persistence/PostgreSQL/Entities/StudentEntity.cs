using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class StudentEntity : BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public StudentDetailEntity? StudentDetail { get; set; }
        public ICollection<PollInstanceEntity> PollInstances { get; set; } = [];
        public ICollection<CohortEntity> Cohorts { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}