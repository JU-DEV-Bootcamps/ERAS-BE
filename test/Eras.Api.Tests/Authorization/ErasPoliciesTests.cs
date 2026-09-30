using System.Security.Claims;
using System.Text.Json;

using Eras.Domain.Entities.UserManagement;
using Eras.Infrastructure.Authorization;

using Microsoft.IdentityModel.JsonWebTokens;

using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Eras.Api.Tests.Authorization;

/// <summary>
/// Exercises the real Keycloak.AuthServices.Authorization wiring (ErasPolicies + DI
/// registration) against a hand-built ClaimsPrincipal carrying a `resource_access`
/// claim, the same shape a validated Keycloak access token produces. No HTTP pipeline
/// or running Keycloak instance is needed: ResourceAccessRequirementHandler reads
/// `resource_access` directly from the principal's claims.
/// </summary>
public class ErasPoliciesTests
{
    private const string RolesResource = "public-client";

    private static IAuthorizationService BuildAuthorizationService(KeycloakRoleNames? roleNames = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddKeycloakAuthorization(options =>
        {
            options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
            options.RolesResource = RolesResource;
        });
        services.AddAuthorization(options => ErasPolicies.Configure(options, RolesResource, roleNames));

        return services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
    }

    private static ClaimsPrincipal BuildUser(params string[] roles)
    {
        var resourceAccessJson = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            [RolesResource] = new { roles },
        });

        var identity = new ClaimsIdentity(
            [new Claim("resource_access", resourceAccessJson, JsonClaimValueTypes.Json)],
            authenticationType: "Test");

        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task AdminOnly_Succeeds_ForAdminRole()
    {
        var service = BuildAuthorizationService();
        var user = BuildUser("ERAS Administrator");

        var result = await service.AuthorizeAsync(user, ErasPolicies.AdminOnly);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task AdminOnly_Fails_ForProfessionalRole()
    {
        var service = BuildAuthorizationService();
        var user = BuildUser("ERAS Professional");

        var result = await service.AuthorizeAsync(user, ErasPolicies.AdminOnly);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task AdminOrOfficer_Succeeds_ForOfficerRole()
    {
        var service = BuildAuthorizationService();
        var user = BuildUser("ERAS Student Services Officer");

        var result = await service.AuthorizeAsync(user, ErasPolicies.AdminOrOfficer);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task AdminOrOfficer_Fails_ForProfessionalRole()
    {
        var service = BuildAuthorizationService();
        var user = BuildUser("ERAS Professional");

        var result = await service.AuthorizeAsync(user, ErasPolicies.AdminOrOfficer);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task AnyErasRole_Succeeds_ForProfessionalRole()
    {
        var service = BuildAuthorizationService();
        var user = BuildUser("ERAS Professional");

        var result = await service.AuthorizeAsync(user, ErasPolicies.AnyErasRole);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task AnyErasRole_Fails_ForUserWithNoRoles()
    {
        var service = BuildAuthorizationService();
        var user = BuildUser();

        var result = await service.AuthorizeAsync(user, ErasPolicies.AnyErasRole);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task AdminOnly_Succeeds_ForConfiguredEnvironmentSpecificAdminName()
    {
        var roleNames = new KeycloakRoleNames(
            Administrator: "admin",
            Officer: "Student Services Officer",
            Professional: "Professional");
        var service = BuildAuthorizationService(roleNames);
        var user = BuildUser("admin");

        var result = await service.AuthorizeAsync(user, ErasPolicies.AdminOnly);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task AdminOnly_Fails_ForLocalDevRoleName_WhenConfiguredWithDifferentNames()
    {
        var roleNames = new KeycloakRoleNames(
            Administrator: "admin",
            Officer: "Student Services Officer",
            Professional: "Professional");
        var service = BuildAuthorizationService(roleNames);
        var user = BuildUser("ERAS Administrator");

        var result = await service.AuthorizeAsync(user, ErasPolicies.AdminOnly);

        Assert.False(result.Succeeded);
    }
}
