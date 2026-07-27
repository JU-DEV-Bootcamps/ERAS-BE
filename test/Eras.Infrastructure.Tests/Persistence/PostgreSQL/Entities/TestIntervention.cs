
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Entities;

internal sealed class TestIntervention : Intervention
{
    public override InterventionKind Kind => InterventionKind.Group;
}