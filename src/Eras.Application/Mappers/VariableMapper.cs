using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class VariableMapper
    {
        public static Variable ToDomain(this VariableDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            return new Variable
            {
                Name = Dto.Name,
                Position = Dto.Position,
                Audit = Dto.Audit?? new AuditInfo(),
                Version = Dto.Version,
            };
        }
        public static VariableDTO ToDto(this Variable Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            return new VariableDTO
            {
                Name = Domain.Name,
                Position = Domain.Position,
                Audit = Domain.Audit,
                Version = Domain.Version,
            };
        }
    }
}