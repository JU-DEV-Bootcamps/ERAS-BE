namespace Eras.Application.Features.ErasUsers.Models;

/// <summary>
/// PUT body for admin-managed profile fields. Deliberately excludes the Keycloak-synced
/// identity fields (first name, last name, email, role), which are never editable here.
/// </summary>
public sealed class UpdateUserProfileRequest
{
    public string? EmployeeId { get; set; }
    public string? Department { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string? About { get; set; }
}
