namespace Eras.Domain.Entities.AssessmentManagement.StatusManagement;

public static class StatusTransitionsConfig
{
    public static void Configure()
    {
        StatusTransitions<AssessmentStatus>.Configure(new()
        {
            [AssessmentStatus.Remitted] = new[] { AssessmentStatus.InProgress, AssessmentStatus.Finalized },
            [AssessmentStatus.InProgress] = new[] { AssessmentStatus.Finalized },
            [AssessmentStatus.Finalized] = Array.Empty<AssessmentStatus>(),
        });

        StatusTransitions<InterventionStatus>.Configure(new()
        {
            [InterventionStatus.Remitted] = new[] { InterventionStatus.InProgress, InterventionStatus.Finalized },
            [InterventionStatus.InProgress] = new[] { InterventionStatus.Finalized },
            [InterventionStatus.Finalized] = Array.Empty<InterventionStatus>(),
        });
    }
}
