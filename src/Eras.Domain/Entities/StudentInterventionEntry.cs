using Eras.Domain.Common;

namespace Eras.Domain.Entities;

/// <summary>
/// A risk identified for a student or a support the student received, recorded by a user.
/// It is linked to <see cref="Student"/> by <see cref="StudentId"/> and is independent of
/// <see cref="StudentProfile"/>, so students without a profile can have entries.
/// </summary>
public class StudentInterventionEntry : BaseEntity, IAuditableEntity
{
    public int StudentId { get; set; }

    public StudentInterventionKind Kind { get; set; }

    public StudentInterventionType Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateOnly EntryDate { get; set; }

    /// <summary>Display name of the creator at creation time; the identity is <c>Audit.CreatedBy</c> (Keycloak sub).</summary>
    public string CreatedByName { get; set; } = string.Empty;

    /// <summary>Set when the entry is deleted; <c>Audit.ModifiedBy</c> and <c>ModifiedAt</c> then hold who deleted it and when.</summary>
    public bool IsDeleted { get; set; } = false;

    public AuditInfo Audit { get; set; } = default!;
}
