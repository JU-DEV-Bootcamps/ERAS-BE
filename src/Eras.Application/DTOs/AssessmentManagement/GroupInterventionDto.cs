namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record GroupInterventionDto : InterventionDto
{
    public string? Area { get; init; }
    public IReadOnlyCollection<int> ParticipantIds { get; init; } = Array.Empty<int>();
}
