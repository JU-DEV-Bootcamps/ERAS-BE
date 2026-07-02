using Eras.Domain.Enums;

namespace Eras.Application.Models.Response.Users;

public class GetCurrentUserRoleResponse
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ErasRole Role { get; set; }
}