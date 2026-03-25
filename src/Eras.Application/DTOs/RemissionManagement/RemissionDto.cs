using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.DTOs.RemissionManagement;

public sealed record RemissionDto
{
    public Guid? Id { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public required string CreatedBy { get; init; }
    public required string Service { get; init; }

    public string? AssignedProfessional { get; init; }

    public required IReadOnlyCollection<Guid> StudentIds { get; init; }

    public string? Diagnosis { get; init; }
    public string? Objective { get; init; }
    public string? Comments { get; init; }

    public InterventionPlanDto? Plan { get; init; }

    public required RemissionStatus Status { get; init; }

    public IReadOnlyCollection<InterventionDto> Interventions { get; init; } = Array.Empty<InterventionDto>();
}
