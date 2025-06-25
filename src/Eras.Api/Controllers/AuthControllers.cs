using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Infrastructure;
using Eras.Infrastructure.External.KeycloakClient;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[AllowAnonymous]
[Route("api/v1/auth")]
[ApiController]
[ExcludeFromCodeCoverage]
public class AuthController(
    IKeycloakAuthService<TokenResponse> KeycloakAuthService, ILogger<AuthController> Logger) : ControllerBase
{
    private readonly IKeycloakAuthService<TokenResponse> _keycloakAuthService = KeycloakAuthService;
    private readonly ILogger<AuthController> _logger = Logger;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest Request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user {Username}", Request.Username);

            TokenResponse token = await _keycloakAuthService.LoginAsync(Request.Username, Request.Password);

            _logger.LogInformation("User {Username} logged in successfully", Request.Username);

            return Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}", Request.Username);
            return Unauthorized(ex.Message);
        }
    }
}
