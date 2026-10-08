using System.Security.Claims;
using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Infrastructure.External.KeycloakClient;

using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Eras.Infrastructure.Authentication;

public sealed class CurrentUserService(
    IHttpContextAccessor HttpContextAccessor,
    IOptions<KeycloakAuthorizationOptions> AuthorizationOptions
) : ICurrentUserService
{
    private ClaimsPrincipal? User => HttpContextAccessor.HttpContext?.User;

    public string? Sub =>
        User?.FindFirst("sub")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Email =>
        User?.FindFirst("email")?.Value ?? User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? FirstName =>
        User?.FindFirst("given_name")?.Value ?? User?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? LastName =>
        User?.FindFirst("family_name")?.Value ?? User?.FindFirst(ClaimTypes.Surname)?.Value;

    public string? Name =>
        User?.FindFirst("name")?.Value ?? User?.FindFirst(ClaimTypes.Name)?.Value;

    /// <summary>
    /// Reads the client roles directly from the `resource_access` claim, the same
    /// way ResourceAccessRequirementHandler (the authorization policy) does. This does
    /// NOT rely on the optional Keycloak.AuthServices role-claims transformation
    /// mapping them onto ClaimTypes.Role, which is not guaranteed to have run for
    /// every request pipeline.
    /// </summary>
    public IReadOnlyCollection<string> Roles
    {
        get
        {
            var resourceAccessClaim = User?.FindFirst("resource_access")?.Value;
            if (string.IsNullOrEmpty(resourceAccessClaim))
                return [];

            var resourceAccess = JsonSerializer.Deserialize<Dictionary<string, Resource>>(resourceAccessClaim);
            var rolesResource = AuthorizationOptions.Value.RolesResource;

            if (resourceAccess is null || rolesResource is null)
                return [];

            return resourceAccess.TryGetValue(rolesResource, out Resource? resource)
                ? resource.roles
                : [];
        }
    }
}
