namespace Eras.Domain.Entities.AssessmentManagement;

public sealed class Assessment : BaseEntity
{
    public DateTime CreatedAtUtc { get; init; }

    public required string CreatedBy { get; init; }
    public required string Service { get; init; }

    public string? AssignedProfessional { get; init; }

    /// <summary>
    /// Supports one or more students, since Marcel mentioned possible group cases.
    /// </summary>
    public required int[] StudentIds { get; init; }

    public string? Diagnosis { get; init; }
    public string? Objective { get; init; }
    public string? Comments { get; init; }

    public InterventionPlan? Plan { get; init; }

    public required AssessmentStatus Status { get; init; }

    public IReadOnlyCollection<Intervention> Interventions { get; init; } = Array.Empty<Intervention>();
}