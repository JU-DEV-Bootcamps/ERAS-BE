using System;

using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Mappers
{
    public static class PollMapper
    {
        public static Poll ToDomain(this PollDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ICollection<Component> components = dto.Components?.Select(c => c.ToDomain()).ToList() ?? [];
            return new Poll
            {
                Id = dto.Id,
                Name = dto.Name,
                Version = dto.Version,
                Uuid = dto.Uuid,
                Components = components,
                Audit = dto.Audit
            };
        }
        public static PollDTO ToDto(this Poll domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            ICollection<ComponentDTO> components = domain.Components?.Select(c => c.ToDto()).ToList() ?? [];
            return new PollDTO
            {
                Id = domain.Id,
                Name = domain.Name,
                Uuid = domain.Uuid,
                Version = domain.Version,
                Components = components,
                Audit = domain.Audit
            };
        }
    }
}
