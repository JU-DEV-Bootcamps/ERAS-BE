using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class ComponentMapper
    {
        public static Component ToDomain(this ComponentEntity entity)
        {
            return new Component
            {
                Id = entity.Id,
                Name = entity.Name,
                Audit = entity.Audit
            };
        }

        public static ComponentEntity ToPersistence(this Component model)
        {
            return new ComponentEntity
            {
                Id = model.Id,
                Name = model.Name,
                Audit = model.Audit
            };
        }
    }
}