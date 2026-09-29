using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Eras.Api.Middleware;

/// <summary>
/// Wraps the default authorization result handler to: (1) return a clear, consistent
/// JSON body on 403 (insufficient role) instead of an empty response, and (2) log
/// every denied access attempt for auditing, per the RBAC epic's requirements.
/// Authentication failures (401 - no/invalid token) are left to the default handler.
/// </summary>
public sealed class ErasAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    private readonly ILogger<ErasAuthorizationMiddlewareResultHandler> _logger;

    public ErasAuthorizationMiddlewareResultHandler(ILogger<ErasAuthorizationMiddlewareResultHandler> Logger)
    {
        _logger = Logger;
    }

    public async Task HandleAsync(
        RequestDelegate Next,
        HttpContext Context,
        AuthorizationPolicy Policy,
        PolicyAuthorizationResult AuthorizeResult)
    {
        if (!AuthorizeResult.Forbidden)
        {
            await _defaultHandler.HandleAsync(Next, Context, Policy, AuthorizeResult);
            return;
        }

        string userId = Context.User.FindFirst("sub")?.Value
            ?? Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? "unknown";
        string email = Context.User.FindFirst("email")?.Value ?? "unknown";
        string? endpointName = Context.GetEndpoint()?.DisplayName;

        _logger.LogWarning(
            "Unauthorized access attempt: user {UserId} ({Email}) was denied {Method} {Path} ({Endpoint}).",
            userId,
            email,
            Context.Request.Method,
            Context.Request.Path,
            endpointName ?? "unknown endpoint");

        Context.Response.StatusCode = StatusCodes.Status403Forbidden;
        Context.Response.ContentType = "application/json";

        await Context.Response.WriteAsJsonAsync(new
        {
            statusCode = StatusCodes.Status403Forbidden,
            message = "You do not have permission to access this resource.",
        });
    }
}
