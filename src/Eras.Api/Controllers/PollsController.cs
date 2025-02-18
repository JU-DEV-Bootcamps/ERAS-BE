using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PollsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPolls()
        {
            return Ok("All list of polls in our DB");
        }
    }
}
