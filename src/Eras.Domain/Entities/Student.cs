using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    public class Student : BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public StudentDetail? StudentDetail { get; set; }
        public ICollection<PollInstance> PollInstances { get; set; } = [];
        public ICollection<Cohort> Cohorts { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}


