using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class VariableMapper
    {
        public static Variable ToDomain(this VariableDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            return new Variable
            {
                Name = dto.Name,
                Audit = dto.Audit,
            };
        }
        public static VariableDTO ToDto(this Variable domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            return new VariableDTO
            {
                Name = domain.Name,
                Audit = domain.Audit,
            };
        }
    }
}
