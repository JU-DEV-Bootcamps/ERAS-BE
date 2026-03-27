namespace Eras.Domain.Entities.AssessmentManagement;

public sealed record InterventionPlan
{
    public int? SessionsPerWeek { get; init; }
    public string? ScheduleNotes { get; init; }
}