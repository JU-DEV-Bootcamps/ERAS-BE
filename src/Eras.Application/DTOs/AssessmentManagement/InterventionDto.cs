namespace Eras.Application.DTOs.AssessmentManagement;

public abstract record InterventionDto
{
    public Guid? Id { get; init; }

    public required DateTime DateUtc { get; init; }
    public string? ActivityType { get; init; }
    public string? Professional { get; init; }
    public string? Comments { get; init; }
    public IReadOnlyCollection<string> Attachments { get; init; } = Array.Empty<string>();
}
