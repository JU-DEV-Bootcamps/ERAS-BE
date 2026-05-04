using Eras.Domain.Common;

namespace Eras.Domain.Entities.AssessmentManagement;

public abstract class Intervention : BaseEntity
{
    public required DateTime DateUtc { get; init; }
    public string? ActivityType { get; init; }
    public string? Professional { get; init; }
    public string? Comments { get; init; }

    /// <summary>
    /// For now, keep attachments simple.
    /// Could later become a richer child entity.
    /// </summary>
    public IReadOnlyCollection<string> Attachments { get; init; } = Array.Empty<string>();

    public abstract InterventionKind Kind { get; }
}