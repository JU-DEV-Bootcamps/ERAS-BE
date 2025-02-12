using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class VariableMapper
    {
        public static Variable ToDomain(this VariableEntity entity)
        {
            return new Variable
            {
                Id = entity.Id,
                Name = entity.Name,
                Audit = entity.Audit
            };
        }

        public static VariableEntity ToPersistence(this Variable model)
        {
            return new VariableEntity
            {
                Id = model.Id,
                Name = model.Name,
                Audit = model.Audit
            };
        }
    }
}