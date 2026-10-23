using System.Security.Claims;
using System.Text.Json;

using Eras.Infrastructure.Authentication;

using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Authentication;

public class CurrentUserServiceTests
{
    private const string RolesResource = "public-client";

    private static CurrentUserService CreateService(ClaimsPrincipal? user)
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock
            .Setup(A => A.HttpContext)
            .Returns(user is null ? null! : new DefaultHttpContext { User = user });

        var options = Options.Create(new KeycloakAuthorizationOptions
        {
            RolesResource = RolesResource,
        });

        return new CurrentUserService(httpContextAccessorMock.Object, options);
    }

    private static ClaimsPrincipal BuildUser(string resource, params string[] roles)
    {
        var resourceAccessJson = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            [resource] = new { roles },
        });

        var identity = new ClaimsIdentity(
            [
                new Claim("sub", "user-sub-123"),
                new Claim("email", "user@test.com"),
                new Claim("given_name", "First"),
                new Claim("family_name", "Last"),
                new Claim("resource_access", resourceAccessJson, JsonClaimValueTypes.Json),
            ],
            authenticationType: "Test");

        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public void Roles_ReadsRolesFromResourceAccessClaim_ForConfiguredResource()
    {
        var user = BuildUser(RolesResource, "ERAS Professional");
        var service = CreateService(user);

        Assert.Equal(["ERAS Professional"], service.Roles);
    }

    [Fact]
    public void Roles_IgnoresRolesUnderADifferentResource()
    {
        // Regression test: this previously relied on ClaimTypes.Role claims that the
        // Keycloak.AuthServices claims transformation does not reliably populate,
        // silently resolving every real user to Guest.
        var user = BuildUser("some-other-client", "ERAS Administrator");
        var service = CreateService(user);

        Assert.Empty(service.Roles);
    }

    [Fact]
    public void Roles_ReturnsEmpty_WhenNoResourceAccessClaimPresent()
    {
        var identity = new ClaimsIdentity([new Claim("sub", "user-sub-123")], "Test");
        var service = CreateService(new ClaimsPrincipal(identity));

        Assert.Empty(service.Roles);
    }

    [Fact]
    public void Roles_ReturnsEmpty_WhenNoHttpContext()
    {
        var service = CreateService(null);

        Assert.Empty(service.Roles);
    }

    [Fact]
    public void IdentityClaims_AreReadFromTheirOwnClaimNames()
    {
        var user = BuildUser(RolesResource, "ERAS Professional");
        var service = CreateService(user);

        Assert.Equal("user-sub-123", service.Sub);
        Assert.Equal("user@test.com", service.Email);
        Assert.Equal("First", service.FirstName);
        Assert.Equal("Last", service.LastName);
    }
}
