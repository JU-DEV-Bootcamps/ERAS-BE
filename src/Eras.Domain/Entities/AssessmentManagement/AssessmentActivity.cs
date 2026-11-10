using Eras.Domain.Common;

namespace Eras.Domain.Entities.AssessmentManagement;

/// <summary>
/// One event of the activity history of an <see cref="Assessment"/>. The text shown to users is built
/// from <see cref="Type"/> and the statuses when the history is read; the comment text is never stored.
/// </summary>
public class AssessmentActivity : BaseEntity
{
    public int AssessmentId { get; set; }

    public AssessmentActivityType Type { get; set; }

    /// <summary>Set only for <see cref="AssessmentActivityType.StatusChanged"/>.</summary>
    public AssessmentStatus? PreviousStatus { get; set; }

    /// <summary>Set only for <see cref="AssessmentActivityType.StatusChanged"/>.</summary>
    public AssessmentStatus? NewStatus { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    /// <summary>Keycloak sub of the user who acted.</summary>
    public string ActorSub { get; set; } = string.Empty;

    /// <summary>Display name of the user at the time of the event; it does not change if the user is renamed.</summary>
    public string ActorName { get; set; } = string.Empty;
}
