using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using System;

namespace Eras.Application.Mappers
{
    public static class PollMapper
    {
        public static Poll ToDomain (this PollDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            ICollection<Component> components = Dto.Components?.Select(C => C.ToDomain()).ToList() ?? [];
            ICollection<PollVersion> pollVersions = Dto.PollVersions?.Select(Pv => Pv.ToDomain()).ToList() ?? [];
            return new Poll { 
                Id = Dto.Id,
                Name = Dto.Name,
                Uuid = Dto.Uuid,
                Components = components,
                PollVersions = pollVersions,
                Audit = Dto.Audit?? new AuditInfo() { 
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                }
            };
        }
        public static PollDTO ToDto(this Poll Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            ICollection<ComponentDTO> components = Domain.Components?.Select(C => C.ToDto()).ToList() ?? [];
            ICollection<PollVersionDTO> pollVersions = Domain.PollVersions?.Select(Pv => Pv.ToDto()).ToList() ?? [];
            return new PollDTO {
                Id = Domain.Id,
                Name = Domain.Name,
                Uuid= Domain.Uuid,
                Components = components,
                PollVersions = pollVersions,
                Audit = Domain.Audit
            };
        }
    }
}
