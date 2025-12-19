using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class JUProfessionalMapper
    {
        public static JUProfessional ToDomain(this JUProfessionalDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);

            return new JUProfessional()
            {
                Id = Dto.Id,
                Name = Dto.Name,
                Uuid = Dto.Uuid,
                Audit = Dto.Audit,
            };
        }

        public static JUProfessionalDTO ToDTO(this JUProfessional Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);

            return new JUProfessionalDTO()
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Uuid = Entity.Uuid,
                Audit = Entity.Audit,
            };
        }
    }
}