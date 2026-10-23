using System.Security.Claims;
using System.Text.Json;

using Eras.Api.Middleware;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Middleware;

public class ErasAuthorizationMiddlewareResultHandlerTests
{
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<ErasAuthorizationMiddlewareResultHandler>> _loggerMock = new();
    private readonly ErasAuthorizationMiddlewareResultHandler _handler;

    public ErasAuthorizationMiddlewareResultHandlerTests()
    {
        _handler = new ErasAuthorizationMiddlewareResultHandler(_loggerMock.Object);
    }

    private static DefaultHttpContext BuildContext(string? sub = "user-sub", string? email = "user@test.com")
    {
        var claims = new List<Claim>();
        if (sub is not null) claims.Add(new Claim("sub", sub));
        if (email is not null) claims.Add(new Claim("email", email));

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")),
        };
        context.Response.Body = new MemoryStream();
        return context;
    }

    [Fact]
    public async Task HandleAsync_Forbidden_WritesJson403AndLogsWarning_WithoutCallingNext()
    {
        var context = BuildContext();
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        bool nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

        await _handler.HandleAsync(next, context, policy, PolicyAuthorizationResult.Forbid());

        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var doc = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal(403, doc.RootElement.GetProperty("statusCode").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(doc.RootElement.GetProperty("message").GetString()));

        _loggerMock.Verify(
            L => L.Log(
                Microsoft.Extensions.Logging.LogLevel.Warning,
                It.IsAny<Microsoft.Extensions.Logging.EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Succeeded_DelegatesToDefaultHandler_CallingNext()
    {
        var context = BuildContext();
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        bool nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };

        await _handler.HandleAsync(next, context, policy, PolicyAuthorizationResult.Success());

        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }
}
