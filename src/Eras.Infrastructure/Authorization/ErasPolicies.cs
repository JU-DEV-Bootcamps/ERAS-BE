using Eras.Domain.Entities.UserManagement;

using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authorization;

namespace Eras.Infrastructure.Authorization;

/// <summary>
/// Centralized ERAS authorization policy names, matching the role matrix
/// from the RBAC feature (Administrator / Student Services Officer / Professional).
/// Controllers reference these constants instead of hardcoding role combinations.
/// </summary>
public static class ErasPolicies
{
    public const string AdminOnly = "RequireErasAdmin";
    public const string AdminOrOfficer = "RequireErasAdminOrOfficer";
    public const string AnyErasRole = "RequireAnyErasRole";

    public static void Configure(AuthorizationOptions Options, string RolesResource, KeycloakRoleNames? RoleNames = null)
    {
        var names = RoleNames ?? new KeycloakRoleNames();

        Options.AddPolicy(AdminOnly, Policy =>
            Policy.RequireResourceRolesForClient(RolesResource, [names.Administrator]));

        Options.AddPolicy(AdminOrOfficer, Policy =>
            Policy.RequireResourceRolesForClient(
                RolesResource,
                [names.Administrator, names.Officer]));

        Options.AddPolicy(AnyErasRole, Policy =>
            Policy.RequireResourceRolesForClient(
                RolesResource,
                [names.Administrator, names.Officer, names.Professional]));
    }
}
