
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class ProfessionalMapper
    {
        public static Professional ToDomain(this ProfessionalEntity Entity) => new Professional
        {
            Id = Entity.Id,
            Uuid = Entity.Uuid,
            Audit = Entity.Audit,
        };
        public static ProfessionalEntity ToPersistence(this Professional Model)
        {
            return new ProfessionalEntity
            {
                Id = Model.Id,
                Uuid = Model.Uuid,
                Audit = Model.Audit,
            };
        }
    }
}
