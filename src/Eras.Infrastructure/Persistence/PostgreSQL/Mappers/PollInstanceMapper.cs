using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class PollInstanceMapper
    {
        public static PollInstance ToDomain(this PollInstanceEntity entity)
        {
            return new PollInstance
            {
                Id = entity.Id,
                Uuid = entity.Uuid,
                Student = entity.Student.ToDomain(),
                Audit = entity.Audit
            };
        }

        public static PollInstanceEntity ToPersistence(this PollInstance model)
        {
            return new PollInstanceEntity
            {
                Id = model.Id,
                Uuid = model.Uuid,
                StudentId = model.Student.Id,
                Audit = model.Audit
            };
        }
    }
}