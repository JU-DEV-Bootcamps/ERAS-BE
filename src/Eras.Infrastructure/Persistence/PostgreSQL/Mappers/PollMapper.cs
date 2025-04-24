using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class PollMapper 
    {
        public static Poll ToDomain(this PollEntity Entity)
        {
            return new Poll
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Version = Entity.Version,
                Uuid = Entity.Uuid,
                Audit = Entity.Audit
            };
        }

        public static PollEntity ToPersistence(this Poll Model)
        {
            return new PollEntity
            {
                Id = Model.Id,
                Name = Model.Name,
                Version = Model.Version,
                Uuid = Model.Uuid,
                Audit = Model.Audit
            };
        }
    }
}
