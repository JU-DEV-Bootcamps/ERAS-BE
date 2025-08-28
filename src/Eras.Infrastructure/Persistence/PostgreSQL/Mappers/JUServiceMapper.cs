using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class JUServiceMapper
    {
        public static JUService ToDomain(this JUServiceEntity Entity)
        {
            return new JUService
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Audit = Entity.Audit,
            };
        }

        public static JUServiceEntity ToPersistence(this JUService Model)
        {
            return new JUServiceEntity 
            {
                Id = Model.Id,
                Name = Model.Name,
                Audit = Model.Audit,
            };
        }
    }
}
