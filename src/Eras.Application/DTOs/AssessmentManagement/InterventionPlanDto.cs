namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record InterventionPlanDto
{
    public Guid? Id { get; init; }

    public int? SessionsPerWeek { get; init; }
    public string? ScheduleNotes { get; init; }
}
