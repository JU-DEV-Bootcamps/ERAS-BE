using System.Security.Claims;

using Eras.Application.Services;

namespace Eras.Api.Middleware;

/// <summary>
/// Captures the authenticated caller's identity once per request into the request-scoped
/// <see cref="UserIdentityProvider"/>, so downstream code (controllers, application services,
/// repositories) can read "who is calling" via
/// <see cref="Eras.Application.Contracts.Services.IUserIdentityProvider"/> instead of reading
/// claims directly off <c>HttpContext.User</c>. Must run after <c>UseAuthentication</c>, since
/// that's what populates <c>HttpContext.User</c> in the first place.
/// </summary>
public sealed class UserIdentityMiddleware(RequestDelegate Next)
{
    private readonly RequestDelegate _next = Next;

    public async Task InvokeAsync(HttpContext Context, UserIdentityProvider IdentityProvider)
    {
        string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User.FindFirstValue("sub")
            ?? UserIdentityProvider.UnknownUserId;

        IdentityProvider.SetUserId(userId);

        await _next(Context);
    }
}
