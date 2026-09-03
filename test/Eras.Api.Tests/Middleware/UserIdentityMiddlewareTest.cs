using System.Security.Claims;

using Eras.Api.Middleware;
using Eras.Application.Services;

using Microsoft.AspNetCore.Http;

namespace Eras.Api.Tests.Middleware;

public class UserIdentityMiddlewareTest
{
    [Fact]
    public async Task InvokeAsync_Should_SetUserId_FromNameIdentifierClaim_BeforeCallingNextAsync()
    {
        // Arrange
        var provider = new UserIdentityProvider();
        string? observedUserId = null;
        var middleware = new UserIdentityMiddleware(_ =>
        {
            observedUserId = provider.UserId;
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "user-42")]))
        };

        // Act
        await middleware.InvokeAsync(context, provider);

        // Assert — the provider is already populated by the time next() runs.
        Assert.Equal("user-42", observedUserId);
        Assert.Equal("user-42", provider.UserId);
    }

    [Fact]
    public async Task InvokeAsync_Should_FallBackToSubClaim_When_NameIdentifierMissingAsync()
    {
        // Arrange
        var provider = new UserIdentityProvider();
        var middleware = new UserIdentityMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "user-99")]))
        };

        // Act
        await middleware.InvokeAsync(context, provider);

        // Assert
        Assert.Equal("user-99", provider.UserId);
    }

    [Fact]
    public async Task InvokeAsync_Should_DefaultToUnknown_When_NoIdentityClaimsPresentAsync()
    {
        // Arrange
        var provider = new UserIdentityProvider();
        var middleware = new UserIdentityMiddleware(_ => Task.CompletedTask);
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) };

        // Act
        await middleware.InvokeAsync(context, provider);

        // Assert
        Assert.Equal(UserIdentityProvider.UnknownUserId, provider.UserId);
    }

    [Fact]
    public async Task InvokeAsync_Should_CallNextAsync()
    {
        // Arrange
        var provider = new UserIdentityProvider();
        bool nextCalled = false;
        var middleware = new UserIdentityMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context, provider);

        // Assert
        Assert.True(nextCalled);
    }
}
