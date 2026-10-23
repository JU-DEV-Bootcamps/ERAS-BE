namespace Eras.Domain.Entities.AssessmentManagement;

public sealed class IndividualIntervention : Intervention
{
    public override InterventionKind Kind => InterventionKind.Individual;
}