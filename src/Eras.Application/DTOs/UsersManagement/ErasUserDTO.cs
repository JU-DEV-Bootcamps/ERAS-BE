using Eras.Domain.Common;

namespace Eras.Application.DTOs.UsersManagement;
public class ErasUserDTO
{
    public int? Id { get; set;}
    public string? Sub { get; set; }
    public required string Email { get; set; } = string.Empty;
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required string Role { get; set; } = string.Empty;
    public bool IsSynced {get; set; } = false;
    public required AuditInfo Audit { get; set; }
}