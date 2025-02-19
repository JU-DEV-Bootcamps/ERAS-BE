using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Mappers
{
    public static class ComponentMapper
    {
        public static Component ToDomain(this ComponentDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ICollection<Variable> variables = dto.Variables?.Select(v => v.ToDomain()).ToList() ?? [];
            return new Component
            {
                Id = default,
                Name = dto.Name,
                Variables = variables,
                Audit = dto.Audit
            };
        }
        public static ComponentDTO ToDto(this Component domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            ICollection<VariableDTO> variables = domain.Variables?.Select(v => v.ToDto()).ToList() ?? [];
            return new ComponentDTO
            {
                Name = domain.Name,
                Variables = variables,
                Audit = domain.Audit
            };
        }
    }
}