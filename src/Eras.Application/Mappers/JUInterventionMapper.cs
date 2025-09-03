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
                Diagnostic = Dto.Diagnostic,
                Objective = Dto.Objective,
                StudentId = Dto.StudentId,
                Remissions = Dto.Remissions.Select((Rem) => Rem.ToDomain()),
                Audit = Dto.Audit,
            };
        }

        public static JUInterventionDTO ToDTO(this JUIntervention Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);

            return new JUInterventionDTO()
            {
                Diagnostic = Entity.Diagnostic,
                Objective = Entity.Objective,
                StudentId = Entity.StudentId,
                Remissions = Entity.Remissions.Select((Rem) => Rem.ToDTO()),
                Audit = Entity.Audit,
            };
        }
    }
}