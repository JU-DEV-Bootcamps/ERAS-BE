using Eras.Domain.Enums;

namespace Eras.Domain.Entities;

public class UserRoleEntity
{
    public string Email { get; set; } = default!; 
    public ErasRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}