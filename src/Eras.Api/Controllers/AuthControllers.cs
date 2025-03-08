using System.Diagnostics.CodeAnalysis;
using Eras.Infrastructure.External.KeycloakClient;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ExcludeFromCodeCoverage]
    public class AuthController : ControllerBase
    {
        private readonly KeycloakAuthService _keycloakAuthService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(KeycloakAuthService keycloakAuthService, ILogger<AuthController> logger)
        {
            _keycloakAuthService = keycloakAuthService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for user {Username}", request.Username);

                var token = await _keycloakAuthService.LoginAsync(request.Username, request.Password);

                _logger.LogInformation("User {Username} logged in successfully", request.Username);

                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user {Username}", request.Username);
                return Unauthorized(ex.Message);
            }
        }
    }    
}
