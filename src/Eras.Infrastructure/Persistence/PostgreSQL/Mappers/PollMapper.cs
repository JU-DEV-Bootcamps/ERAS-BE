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
                Uuid = Entity.Uuid,
                Audit = Entity.Audit,
                LastVersion = Entity.LastVersion,
                LastVersionDate = Entity.LastVersionDate,
                ParentId = Entity.ParentId,
            };
        }

        public static PollEntity ToPersistence(this Poll Model)
        {
            return new PollEntity
            {
                Id = Model.Id,
                Name = Model.Name,
                Uuid = Model.Uuid,
                Audit = Model.Audit,
                LastVersion = Model.LastVersion,
                LastVersionDate = Model.LastVersionDate,
                ParentId = Model.ParentId,
            };
        }
    }
}
