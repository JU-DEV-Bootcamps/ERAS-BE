using Eras.Infrastructure.External.KeycloakClient;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly KeycloakAuthService _keycloakAuthService;

        public AuthController(KeycloakAuthService keycloakAuthService)
        {
            _keycloakAuthService = keycloakAuthService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _keycloakAuthService.LoginAsync(request.Username, request.Password);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }    
}
