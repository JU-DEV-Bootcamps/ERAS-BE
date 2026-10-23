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

    /// <summary>
    /// Keycloak sub of whoever created this intervention. Set once at creation time from
    /// <c>IUserIdentityProvider</c> and preserved across edits/replacements; null for
    /// interventions created before this field existed. Drives the by-creator/by-assigned
    /// role-scoped queries (#545) — distinct from <see cref="Professional"/>, which is a
    /// free-text display field, not an identity reference.
    /// </summary>
    public string? CreatedBy { get; set; }
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