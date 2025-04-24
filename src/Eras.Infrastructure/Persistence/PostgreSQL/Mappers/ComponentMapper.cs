using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class ComponentMapper
    {
        public static Component ToDomain(this ComponentEntity Entity)
        {
            return new Component
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Audit = Entity.Audit
            };
        }

        public static ComponentEntity ToPersistence(this Component Model)
        {
            return new ComponentEntity
            {
                Id = Model.Id,
                Name = Model.Name,
                Audit = Model.Audit
            };
        }
    }
}
