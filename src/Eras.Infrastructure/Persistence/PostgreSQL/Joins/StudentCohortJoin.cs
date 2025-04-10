using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Joins
{
    public class StudentCohortJoin : BaseEntity
    {
        public int StudentId { get; set; }
        public StudentEntity Student { get; set; } = default!;
        public int CohortId { get; set; }
        public CohortEntity Cohort { get; set; } = default!;
    }
}
