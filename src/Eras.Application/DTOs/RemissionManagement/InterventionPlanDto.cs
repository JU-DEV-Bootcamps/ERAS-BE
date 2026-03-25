namespace Eras.Application.DTOs.RemissionManagement;

public sealed record InterventionPlanDto
{
    public Guid? Id { get; init; }

    public int? SessionsPerWeek { get; init; }
    public string? ScheduleNotes { get; init; }
}
