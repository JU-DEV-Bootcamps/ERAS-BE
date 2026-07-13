namespace Eras.Domain.Entities.AssessmentManagement.StatusManagement;

public static class StatusTransitions<TStatus> where TStatus : struct, Enum
{
    private static Dictionary<TStatus, TStatus[]> AllowedTransitions = new();
    
    public static void Configure(Dictionary<TStatus, TStatus[]> transitions)
    {
        AllowedTransitions = transitions;
    }

    public static bool CanTransition(TStatus from, TStatus to)
    {
        if (from.Equals(to)) return true;
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }
}
