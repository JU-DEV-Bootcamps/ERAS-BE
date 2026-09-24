using Eras.Domain.Common;

namespace Eras.Domain.Entities.AssessmentManagement;

public class Intervention : BaseEntity
{
    public required DateTime DateUtc { get; init; }
    public string? Activity { get; set; }
    public string? Area { get; set; }
    public int? NumberOfParticipants { get; set; }
    public string? Professional { get; set; }
    public string? Comments { get; set; }
    public required IReadOnlyCollection<int> StudentIds { get; set; } = Array.Empty<int>();

    public IReadOnlyDictionary<int, bool> Attendance { get; set; } = new Dictionary<int, bool>();

    public InterventionMode Mode { get; set; }
    public InterventionStatus Status { get; set; } = InterventionStatus.Remitted;
    public string? Remarks { get; set; }

    public virtual InterventionKind Kind { get; }

    public IReadOnlyCollection<string> Attachments { get; set; } = Array.Empty<string>();

    public IReadOnlyCollection<string> AttachmentHashes { get; set; } = Array.Empty<string>();
    public double? RiskLevel { get; set; }
    public InterventionLevel RiskLevelName { get; set; } = InterventionLevel.Medium;
    public InterventionLevel? EndRiskLevelName { get; set; }
}