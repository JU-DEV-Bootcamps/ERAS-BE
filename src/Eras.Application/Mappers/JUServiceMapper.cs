using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class JUServiceMapper
    {
        public static JUService ToDomain(this JUServiceDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);

            return new JUService()
            {
                Name = Dto.Name,
                Audit = Dto.Audit,
            };
        }

        public static JUServiceDTO ToDTO(this JUService Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);

            return new JUServiceDTO()
            {
                Name = Entity.Name,
                Audit = Entity.Audit,
            };
        }
    }
}