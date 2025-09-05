
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
            Student = Entity.Student?.ToDomain(),
            RemissionIds = Entity.RemissionIds,
            Remissions = Entity.Remissions.Select(Rem => Rem.ToDomain()).ToList() ?? [],
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
                Student = Model.Student?.ToPersistence(),
                Remissions = Model.Remissions.Select(Rem => Rem.ToPersistence()).ToList(),
                Audit = Model.Audit,
            };
        }
    }
}
