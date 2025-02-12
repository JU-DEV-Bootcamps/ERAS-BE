using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class CohortMapper
    {
        public static Cohort ToDomain(this CohortEntity entity)
        {
            return new Cohort
            {
                Id = entity.Id,
                Name = entity.Name,
                CourseCode = entity.CourseCode,
                Audit = entity.Audit,
            };
        }

        public static CohortEntity ToPersistence(this Cohort model)
        {
            return new CohortEntity
            {
                Id = model.Id,
                Name = model.Name,
                CourseCode = model.CourseCode,
                Audit = model.Audit,
            };
        }
    }
}