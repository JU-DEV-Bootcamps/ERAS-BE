namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record InterventionPlanDto
{
    public int? SessionsPerWeek { get; init; }
    public string? ScheduleNotes { get; init; }
}
