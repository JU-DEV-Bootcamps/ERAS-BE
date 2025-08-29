using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class ProfessionalMapper
    {
        public static Professional ToDomain(this ProfessionalDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);

            return new Professional()
            {
                Name = Dto.Name,
                Uuid = Dto.Uuid,
                Audit = Dto.Audit,
            };
        }

        public static ProfessionalDTO ToDTO(this Professional Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);

            return new ProfessionalDTO()
            {
                Name = Entity.Name,
                Uuid = Entity.Uuid,
                Audit = Entity.Audit,
            };
        }
    }
}