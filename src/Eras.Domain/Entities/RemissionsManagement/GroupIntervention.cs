
namespace Eras.Domain.Entities.RemissionsManagement;

public sealed class GroupIntervention : Intervention
{
    public override InterventionKind Kind => InterventionKind.Group;

    public string? Area { get; init; }

    /// <summary>
    /// Participants of the group intervention.
    /// Keeping it as student ids for now.
    /// </summary>
    public IReadOnlyCollection<Guid> ParticipantIds { get; init; } = Array.Empty<Guid>();
}