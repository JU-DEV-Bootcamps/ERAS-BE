using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class PollMapper
    {
        public static Poll ToDomain(this PollEntity entity)
        {
            return new Poll
            {
                Id = entity.Id,
                Name = entity.Name,
                Version = entity.Version,
                Uuid = entity.Uuid,
                Audit = entity.Audit
            };
        }

        public static PollEntity ToPersistence(this Poll model)
        {
            return new PollEntity
            {
                Id = model.Id,
                Name = model.Name,
                Version = model.Version,
                Uuid = model.Uuid,
                Audit = model.Audit
            };
        }
    }
}
