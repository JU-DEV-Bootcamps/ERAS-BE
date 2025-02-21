using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Dtos;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Services;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/v1/CosmicLatte/")]
    public class CosmicLatteController : ControllerBase
    {
        private readonly ICosmicLatteAPIService _cosmicLatteService;
        private readonly ILogger<CosmicLatteController> _logger;

        public CosmicLatteController(ICosmicLatteAPIService cosmicLatteService, ILogger<CosmicLatteController> logger)
        {
            _cosmicLatteService = cosmicLatteService;
            _logger = logger;
        }

        [HttpGet("polls/")]
        public async Task<IActionResult> GetPreviewPolls(
        [FromQuery] string name = "",
        [FromQuery] string startDate = "",
        [FromQuery] string endDate = ""
        )
        {
            List<PollDTO> createdPolls = await _cosmicLatteService.ImportAllPolls(name, startDate, endDate);
            if (createdPolls.Count > 0)
            {
                return Ok(createdPolls);
            }
            else
            {
                _logger.LogWarning("An error occurred during the import process.");
                return StatusCode(500, new { status = "error", message = "An error occurred during the import process" });
            }
        }
        [HttpGet("polls/names")]
        public async Task<IActionResult> GetPollsNameList()
        {
            List<PollDataItem> pollList = await _cosmicLatteService.GetPollsNameList();
            return Ok(pollList);
        }

        [HttpPost("polls/")]
        public async Task<IActionResult> PostPolls()
        {
            return Ok("Not implemented yet");
        }

    }
}