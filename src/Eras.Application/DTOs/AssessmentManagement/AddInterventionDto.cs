namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record AddInterventionDto
{
    public required int AssessmentId { get; init; }
    public required InterventionDto Intervention { get; init; }
}