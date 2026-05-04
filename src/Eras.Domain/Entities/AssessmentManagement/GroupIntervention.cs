namespace Eras.Domain.Entities.AssessmentManagement;

public sealed class GroupIntervention : Intervention
{
    public override InterventionKind Kind => InterventionKind.Group;

    public string? Area { get; init; }

    /// <summary>
    /// Participants of the group intervention.
    /// Keeping it as student ids for now.
    /// </summary>
    public IReadOnlyCollection<int> ParticipantIds { get; init; } = Array.Empty<int>();
}