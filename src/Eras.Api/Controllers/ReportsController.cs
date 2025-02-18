using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReportsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetReport()
        {
            return Ok("An specific Report");
        }
    }
}

