using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("get-admin")]

        public IActionResult teste()
        {
            return Ok("You are admin.");
        }

        [HttpGet]
        [Authorize(Roles = "general")]
        [Route("get-general")]
        public IActionResult GetGeneral()
        {
            return Ok("You are general.");
        }
    }
}