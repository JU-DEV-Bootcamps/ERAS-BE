using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class PollMapper
    {
        public static Poll ToDomain (this PollDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            ICollection<Component> components = Dto.Components?.Select(C => C.ToDomain()).ToList() ?? [];
            return new Poll {
                Id = Dto.Id,
                Name = Dto.Name,
                Uuid = Dto.Uuid,
                Audit = Dto.Audit?? new AuditInfo() {
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                },
                LastVersion = Dto.LastVersion,
                LastVersionDate = Dto.LastVersionDate,
                ParentId = Dto.ParentId,
            };
        }
        public static PollDTO ToDto(this Poll Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            ICollection<ComponentDTO> components = Domain.Components?.Select(C => C.ToDto()).ToList() ?? [];
            return new PollDTO {
                Id = Domain.Id,
                Name = Domain.Name,
                Uuid= Domain.Uuid,
                Components = components,
                Audit = Domain.Audit,
                LastVersion = Domain.LastVersion,
                LastVersionDate = Domain.LastVersionDate,
                ParentId = Domain.ParentId,
            };
        }
    }
}
