using Eras.Domain.Common;

namespace Eras.Domain.Entities.UserManagement;

/// <summary>
/// Professional-only data of an ERAS Professional, one row per user. Position is not part of it:
/// it stays on the user record (<see cref="ErasUser.Position"/>), so the general profile keeps reading it.
/// </summary>
public sealed class ProfessionalProfile : BaseEntity, IAuditableEntity
{
    public int ErasUserId { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public string OfficeHours { get; set; } = string.Empty;
    public string[] SubjectsTaught { get; set; } = [];
    public List<ProfessionalCourse> Courses { get; set; } = [];
    public AuditInfo Audit { get; set; } = new AuditInfo()
    {
        CreatedAt = DateTime.UtcNow,
        CreatedBy = "System",
        ModifiedAt = DateTime.UtcNow
    };
}
