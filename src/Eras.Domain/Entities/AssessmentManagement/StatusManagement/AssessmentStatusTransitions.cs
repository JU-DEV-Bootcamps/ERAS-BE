namespace Eras.Domain.Entities.AssessmentManagement.StatusManagement;

public static class AssessmentStatusTransitions
{
    private static readonly Dictionary<AssessmentStatus, AssessmentStatus[]> AllowedTransitions = new()
    {
        [AssessmentStatus.Remitted] = new[] { AssessmentStatus.InProgress },
        [AssessmentStatus.InProgress] = new[] { AssessmentStatus.Finalized },
        [AssessmentStatus.Finalized] = Array.Empty<AssessmentStatus>(),
    };

    public static bool CanTransition(AssessmentStatus from, AssessmentStatus to)
    {
        if (from == to) return true;
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }
}
