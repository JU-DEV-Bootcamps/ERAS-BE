using Eras.Application.Contracts.Infrastructure;
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

        public CosmicLatteController(ICosmicLatteAPIService cosmicLatteService)
        {
            _cosmicLatteService = cosmicLatteService;
        }

        [HttpGet("polls/")]
        public async Task<IActionResult> GetPreviewPolls(
        [FromQuery] string name = "",
        [FromQuery] string startDate = "",
        [FromQuery] string endDate = ""
        )
        {
            var success = await _cosmicLatteService.ImportAllPolls(name, startDate, endDate);
            if (success > 0)
            {
                return Ok(new { status = "successful", message = $"{success} Students imported successfully" });
            }
            else
            {
                return StatusCode(500, new { status = "error", message = "An error occurred during the import process" });
            }
        }

        [HttpPost("polls/")]
        public async Task<IActionResult> PostPolls()
        {
            return Ok("Not implemented yet");
        }

    }
}