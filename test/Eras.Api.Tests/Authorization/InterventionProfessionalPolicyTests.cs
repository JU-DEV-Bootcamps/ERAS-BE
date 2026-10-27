using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

using Eras.Api.Controllers.AssessmentManagement;
using Eras.Infrastructure.Authorization;

using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

using Xunit;

namespace Eras.Api.Tests.Authorization;

/// <summary>
/// Reassigning the professional of an intervention goes through the intervention update endpoint.
/// These tests evaluate the policy declared on that action against principals shaped like a Keycloak token.
/// </summary>
public class InterventionProfessionalPolicyTests
{
    private const string RolesResource = "public-client";

    private static string UpdateInterventionPolicy()
    {
        MethodInfo action = typeof(AssessmentsController).GetMethod("UpdateInterventionAsync")!;
        AuthorizeAttribute attribute = Assert.Single(action.GetCustomAttributes<AuthorizeAttribute>());
        return attribute.Policy!;
    }

    private static async Task<AuthorizationResult> AuthorizeAsync(params string[] Roles)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMetrics();
        services.AddKeycloakAuthorization(options =>
        {
            options.EnableRolesMapping = RolesClaimTransformationSource.ResourceAccess;
            options.RolesResource = RolesResource;
        });
        services.AddAuthorization(options => ErasPolicies.Configure(options, RolesResource, null));
        IAuthorizationService service = services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();

        string resourceAccessJson = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            [RolesResource] = new { roles = Roles },
        });
        var identity = new ClaimsIdentity(
            [new Claim("resource_access", resourceAccessJson, JsonClaimValueTypes.Json)],
            authenticationType: "Test");

        return await service.AuthorizeAsync(new ClaimsPrincipal(identity), UpdateInterventionPolicy());
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-1")]
    public async Task UpdateIntervention_ErasAdministrator_IsAuthorized()
    {
        AuthorizationResult result = await AuthorizeAsync("ERAS Administrator");

        Assert.True(result.Succeeded);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-2")]
    public async Task UpdateIntervention_StudentServicesOfficer_IsAuthorized()
    {
        AuthorizationResult result = await AuthorizeAsync("ERAS Student Services Officer");

        Assert.True(result.Succeeded);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-3")]
    public async Task UpdateIntervention_ErasProfessional_IsAuthorized()
    {
        AuthorizationResult result = await AuthorizeAsync("ERAS Professional");

        Assert.True(result.Succeeded);
    }

    [Fact]
    [Trait("Change", "635-allow-reassign-intervention-professional")]
    [Trait("AC", "635-allow-reassign-intervention-professional/AC-4")]
    public async Task UpdateIntervention_Guest_IsForbidden()
    {
        AuthorizationResult result = await AuthorizeAsync();

        Assert.False(result.Succeeded);
    }
}
