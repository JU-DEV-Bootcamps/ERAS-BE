using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class CohortMapper
    {
        public static Cohort ToDomain(this CohortEntity Entity)
        {
            return new Cohort
            {
                Id = Entity.Id,
                Name = Entity.Name,
                CourseCode = Entity.CourseCode,
                Audit = Entity.Audit,
            };
        }

        public static CohortEntity ToPersistence(this Cohort Model)
        {
            return new CohortEntity
            {
                Id = Model.Id,
                Name = Model.Name,
                CourseCode = Model.CourseCode,
                Audit = Model.Audit,
            };
        }
    }
}
