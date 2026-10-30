using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

using Eras.Infrastructure.Authorization;

using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

using Xunit;

namespace Eras.Api.Tests.Authorization;

/// <summary>
/// Evaluates every <see cref="AuthorizeAttribute"/> that applies to a controller action (the class ones and the
/// action ones are ANDed by ASP.NET) against a principal shaped like a Keycloak token, with the real ErasPolicies.
/// </summary>
internal static class ControllerPolicyEvaluator
{
    private const string RolesResource = "public-client";

    public static MethodInfo Action(Type Controller, string Name) =>
        Controller.GetMethod(Name) ?? throw new InvalidOperationException($"{Controller.Name}.{Name} does not exist.");

    public static async Task<bool> IsAuthorizedAsync(MethodInfo Action, params string[] Roles)
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
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("resource_access", resourceAccessJson, JsonClaimValueTypes.Json)],
            authenticationType: "Test"));

        List<AuthorizeAttribute> attributes = Action.DeclaringType!.GetCustomAttributes<AuthorizeAttribute>(inherit: true)
            .Concat(Action.GetCustomAttributes<AuthorizeAttribute>(inherit: true))
            .ToList();
        Assert.NotEmpty(attributes);

        foreach (AuthorizeAttribute attribute in attributes)
        {
            AuthorizationResult result = await service.AuthorizeAsync(user, attribute.Policy!);
            if (!result.Succeeded) return false;
        }

        return true;
    }
}
