namespace Eras.Application.Contracts.Services;

/// <summary>
/// Placeholder for "the authenticated user calling this request" — available anywhere down the
/// call chain (controllers, application services, repositories) without depending on ASP.NET
/// Core's HttpContext/ClaimsPrincipal directly. Populated once per request by
/// <c>Eras.Api.Middleware.UserIdentityMiddleware</c>, after authentication has run; inject this
/// interface wherever "who is calling" is needed instead of reading claims by hand.
/// </summary>
public interface IUserIdentityProvider
{
    /// <summary>
    /// The current caller's identifier (from the `NameIdentifier`/`sub` claim), or
    /// <c>Eras.Application.Services.UserIdentityProvider.UnknownUserId</c> if the request is
    /// unauthenticated or the identity middleware hasn't run for it.
    /// </summary>
    string UserId { get; }
}
