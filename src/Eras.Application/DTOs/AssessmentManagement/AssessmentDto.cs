using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record AssessmentDto
{
    public int? Id { get; init; }

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public required string CreatedBy { get; init; }
    public required string Service { get; init; }

    public string? AssignedProfessional { get; init; }

    public required int[] StudentIds { get; init; }
    public string[] StudentNames { get; init; } = Array.Empty<string>();

    public string? Diagnosis { get; init; }
    public string? Objective { get; init; }
    public string? Comments { get; init; }

    public InterventionPlanDto? Plan { get; init; }

    public required AssessmentStatus Status { get; init; }

    public IReadOnlyCollection<InterventionDto> Interventions { get; init; } = Array.Empty<InterventionDto>();
}
