using System.Security.Claims;
using Eras.Application.Contracts.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Eras.Infrastructure.Authentication;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? KeycloakSub => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
    ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value 
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

    public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value 
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("given_name")?.Value;

    public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value 
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("family_name")?.Value;
}