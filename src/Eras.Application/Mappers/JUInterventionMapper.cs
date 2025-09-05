using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class JUInterventionMapper
    {
        public static JUIntervention ToDomain(this JUInterventionDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);

            return new JUIntervention()
            {
                Id = Dto.Id,
                Diagnostic = Dto.Diagnostic,
                Objective = Dto.Objective,
                StudentId = Dto.StudentId,
                RemissionIds = Dto.RemissionIds,
                Audit = Dto.Audit,
            };
        }

        public static JUInterventionDTO ToDTO(this JUIntervention Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);

            return new JUInterventionDTO()
            {
                Id = Entity.Id,
                Diagnostic = Entity.Diagnostic,
                Objective = Entity.Objective,
                StudentId = Entity.StudentId,
                RemissionIds = Entity.RemissionIds,
                Audit = Entity.Audit,
            };
        }
    }
}