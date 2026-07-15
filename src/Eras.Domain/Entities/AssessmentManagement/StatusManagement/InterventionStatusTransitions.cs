namespace Eras.Domain.Entities.AssessmentManagement.StatusManagement;

public static class InterventionStatusTransitions
{
    private static readonly Dictionary<InterventionStatus, InterventionStatus[]> AllowedTransitions = new()
    {
        [InterventionStatus.Remitted] = new[] { InterventionStatus.InProgress },
        [InterventionStatus.InProgress] = new[] { InterventionStatus.Finalized },
        [InterventionStatus.Finalized] = Array.Empty<InterventionStatus>(),
    };

    public static bool CanTransition(InterventionStatus from, InterventionStatus to)
    {
        if (from == to) return true;
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }
}
