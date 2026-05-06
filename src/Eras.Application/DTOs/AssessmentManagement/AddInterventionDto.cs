namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record AddInterventionDto
{
    public required Guid AssessmentId { get; init; }
    public required InterventionDto Intervention { get; init; }
}