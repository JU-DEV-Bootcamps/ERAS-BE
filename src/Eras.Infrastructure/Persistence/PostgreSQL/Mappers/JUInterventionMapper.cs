
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class JUInterventionMapper
    {
        public static JUIntervention ToDomain(this JUInterventionEntity Entity) => new JUIntervention
        {
            Id = Entity.Id,
            Diagnostic = Entity.Diagnostic,
            Objective = Entity.Objective,
            StudentId = Entity.StudentId,
            Audit = Entity.Audit,
        };
        public static JUInterventionEntity ToPersistence(this JUIntervention Model)
        {
            return new JUInterventionEntity
            {
                Id = Model.Id,
                Diagnostic = Model.Diagnostic,
                Objective = Model.Objective,
                StudentId = Model.StudentId,
                Remissions = Model.Remissions.Select(Rem => Rem.ToPersistence()).ToList() ?? [],
                Audit = Model.Audit,
            };
        }
    }
}
