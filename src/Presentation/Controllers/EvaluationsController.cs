using Domain.Services;
using Entities;
using Microsoft.AspNetCore.Mvc;

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

        /*

        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Entities.Evaluation>> GetEvaluationById(string id)
        {
            ActionResult<Domain.Entities.Evaluation> Evaluation = await _cosmicLatteService.GetEvaluationById(id);
            if (Evaluation == null) return NotFound($"No se encontró evaluación con id {id}");
            return Ok(Evaluation);
        }

        [HttpGet]
        public async Task<ActionResult<List<Domain.Entities.Evaluation>>> GetEvaluations(
            [FromQuery] string name = null,
            [FromQuery] string startDate = null, // yyyy or yyyy-mm or yyyy-mm-dd
            [FromQuery] string endDate = null // yyyy or yyyy-mm or yyyy-mm-dd
            )
        {
            return Ok(await _cosmicLatteService.GetEvaluations(name, startDate, endDate));
        }
        */


    }
}

