using Eras.Domain.Common;

namespace Eras.Domain.Entities.AssessmentManagement;

public abstract class Intervention : BaseEntity
{
    public required DateTime DateUtc { get; init; }
    public string? Activity { get; init; }
    public string? Area { get; init; }
    public int? NumberOfGuests { get; init; }
    public int? NumberOfParticipants { get; init; }
    public string? Professional { get; init; }
    public string? Comments { get; init; }
    public required IReadOnlyCollection<int> StudentIds { get; init; } = Array.Empty<int>();

    /// <summary>
    /// Maps StudentId → attended. Pre-set for Individual, manual for Group.
    /// Stored as jsonb in PostgreSQL.
    /// </summary>
    public IReadOnlyDictionary<int, bool> Attendance { get; init; } = new Dictionary<int, bool>();

    public InterventionMode Mode { get; init; }
    public InterventionStatus Status { get; init; } = InterventionStatus.Created;
    public string? Remarks { get; init; }

    public IReadOnlyCollection<string> Attachments { get; init; } = Array.Empty<string>();

    public abstract InterventionKind Kind { get; }
}