namespace Eras.Application.DTOs.RemissionManagement;

public sealed record GroupInterventionDto : InterventionDto
{
    public string? Area { get; init; }
    public IReadOnlyCollection<Guid> ParticipantIds { get; init; } = Array.Empty<Guid>();
}
