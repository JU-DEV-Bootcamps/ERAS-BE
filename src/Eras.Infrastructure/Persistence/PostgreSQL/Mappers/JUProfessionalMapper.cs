
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class JUProfessionalMapper
    {
        public static JUProfessional ToDomain(this JUProfessionalEntity Entity) => new JUProfessional
        {
            Id = Entity.Id,
            Uuid = Entity.Uuid,
            Name = Entity.Name,
            Audit = Entity.Audit,
        };
        public static JUProfessionalEntity ToPersistence(this JUProfessional Model)
        {
            return new JUProfessionalEntity
            {
                Id = Model.Id,
                Uuid = Model.Uuid,
                Name = Model.Name,
                Audit = Model.Audit,
            };
        }
    }
}
