using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class ComponentMapper
    {
        public static Component ToDomain(this ComponentDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            ICollection<Variable> variables = Dto.Variables?.Select(V => V.ToDomain()).ToList() ?? [];
            return new Component
            {
                Id = default,
                Name = Dto.Name,
                Variables = variables,
                Audit = Dto.Audit
            };
        }
        public static ComponentDTO ToDto(this Component Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            ICollection<VariableDTO> variables = Domain.Variables?.Select(V => V.ToDto()).ToList() ?? [];
            return new ComponentDTO
            {
                Name = Domain.Name,
                Variables = variables,
                Audit = Domain.Audit
            };
        }
    }
}