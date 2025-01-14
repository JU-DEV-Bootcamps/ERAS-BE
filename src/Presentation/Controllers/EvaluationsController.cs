using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly ICosmicLatteAPIService<CosmicLatteStatus> _cosmicLatteService;

        public EvaluationsController(ICosmicLatteAPIService<CosmicLatteStatus> cosmicLatteService)
        {
            _cosmicLatteService = cosmicLatteService;
        }

        [HttpOptions("/cosmic-latte/status")]
        public async Task<ActionResult<CosmicLatteStatus>> CosmicApiIsHealthy()
        {
            return Ok(await _cosmicLatteService.CosmicApiIsHealthy());
        }
    }
}

