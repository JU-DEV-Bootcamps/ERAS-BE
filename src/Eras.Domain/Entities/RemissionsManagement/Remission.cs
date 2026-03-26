
namespace Eras.Domain.Entities.RemissionsManagement;

public sealed class Remission : BaseEntity
{
    public DateTime CreatedAtUtc { get; init; }

    public required string CreatedBy { get; init; }
    public required string Service { get; init; }

    public string? AssignedProfessional { get; init; }

    /// <summary>
    /// Supports one or more students, since Marcel mentioned possible group cases.
    /// </summary>
    public required IReadOnlyCollection<Guid> StudentIds { get; init; }

    public string? Diagnosis { get; init; }
    public string? Objective { get; init; }
    public string? Comments { get; init; }

    public InterventionPlan? Plan { get; init; }

    public required RemissionStatus Status { get; init; }

    public IReadOnlyCollection<Intervention> Interventions { get; init; } = Array.Empty<Intervention>();
}