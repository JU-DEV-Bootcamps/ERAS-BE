
namespace Eras.Domain.Entities.RemissionsManagement;

public sealed class IndividualIntervention : Intervention
{
    public override InterventionKind Kind => InterventionKind.Individual;
}