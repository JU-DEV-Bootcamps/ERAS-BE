using Eras.Application.Dtos;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/CosmicLatte/")]
    public class CosmicLatteController : ControllerBase
    {
        private readonly ICosmicLatteAPIService _cosmicLatteService;

        public CosmicLatteController(ICosmicLatteAPIService CosmicLatteService)
        {
            _cosmicLatteService = CosmicLatteService;
        }

        [HttpGet("polls/")]
        public async Task<IActionResult> GetPreviewPollsAsync(
        [FromQuery] string EvaluationSetName = "",
        [FromQuery] string StartDate = "",
        [FromQuery] string EndDate = ""
        )
        {
            return Ok(await _cosmicLatteService.GetAllPollsPreview(EvaluationSetName, StartDate, EndDate));
        }

        [HttpPost("polls/")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SavePreviewPollsAsync([FromBody] List<PollDTO> PollsInstances)
        {
            return Ok(await _cosmicLatteService.SavePreviewPolls(PollsInstances));
        }

        [HttpGet("polls/names")]
        public async Task<IActionResult> GetPollsNameListAsync()
        {
            List<PollDataItem> pollList = await _cosmicLatteService.GetPollsNameList();
            return Ok(pollList);
        }

    }
}
