using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using System.ComponentModel;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class VariableMapper
    {
        public static Variable ToDomain(this VariableEntity Entity)
        {
            return new Variable
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Audit = Entity.Audit
            };
        }
        public static VariableEntity ToPersistence(this Variable Model)
        {
            return new VariableEntity
            {
                Id = Model.Id,
                Name = Model.Name,
                Audit = Model.Audit,
                ComponentId = Model.IdComponent
            };
        }
    }
}
