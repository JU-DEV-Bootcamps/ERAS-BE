using Eras.Domain.Common;

namespace Eras.Domain.Entities.UserManagement;
public class ErasUser : BaseEntity, IAuditableEntity
{
    public string? Sub { get; set; }
    public required string Email { get; set; } = string.Empty;
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required string Role { get; set; } = ErasRole.Guest.Label;
    public bool IsSynced {get; set; } = false;
    public AuditInfo Audit { get; set; } = new AuditInfo()
    {
        CreatedAt = DateTime.UtcNow,
        CreatedBy = "System",
        ModifiedAt = DateTime.UtcNow
    };
}