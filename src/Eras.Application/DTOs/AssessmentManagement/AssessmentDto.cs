using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record AssessmentDto
{
    public Guid? Id { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public required string CreatedBy { get; init; }
    public required string Service { get; init; }

    public string? AssignedProfessional { get; init; }

    public required int[] StudentIds { get; init; }

    public string? Diagnosis { get; init; }
    public string? Objective { get; init; }
    public string? Comments { get; init; }

    public InterventionPlanDto? Plan { get; init; }

    public required AssessmentStatus Status { get; init; }

    public IReadOnlyCollection<InterventionDto> Interventions { get; init; } = Array.Empty<InterventionDto>();
}
