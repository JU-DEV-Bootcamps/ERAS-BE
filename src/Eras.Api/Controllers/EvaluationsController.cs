using Eras.Application.Services;
using Eras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly ICosmicLatteAPIService _cosmicLatteService;

        public EvaluationsController(ICosmicLatteAPIService cosmicLatteService)
        {
            _cosmicLatteService = cosmicLatteService;
        }

        [HttpOptions("/cosmic-latte/status")]
        public async Task<ActionResult<CosmicLatteStatus>> CosmicApiIsHealthy()
        {
            return Ok(await _cosmicLatteService.CosmicApiIsHealthy());
        }



        [HttpGet]
        public async Task<ActionResult<List<string>>> GetPolls(
        [FromQuery] string name = "",
        [FromQuery] string startDate = "", // yyyy or yyyy-mm or yyyy-mm-dd
        [FromQuery] string endDate = "" // yyyy or yyyy-mm or yyyy-mm-dd
        )
        {
            return Ok(await _cosmicLatteService.ImportAllPolls(name, startDate, endDate));
        }


        /*
        This should be used only for preview feat, now we are getting and saving data in one step
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetPollById(string id)
        {
            ActionResult<string> Evaluation = await _cosmicLatteService.GetPollById(id);
            if (Evaluation == null) return NotFound($"Not found {id}");
            return Ok(Evaluation.Value);
        }
        */
        /*
        This should be used only for preview feat, now we are getting and saving data in one step
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetPolls(
        [FromQuery] string name = null,
        [FromQuery] string startDate = null, // yyyy or yyyy-mm or yyyy-mm-dd
        [FromQuery] string endDate = null // yyyy or yyyy-mm or yyyy-mm-dd
        )
        {
            return Ok(await _cosmicLatteService.GetPolls(name, startDate, endDate));
        }
        */
    }
}

