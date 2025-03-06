using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Services;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

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
            List<PollDTO> previewPolls = await _cosmicLatteService.GetAllPollsPreview(name, startDate, endDate);
            if (previewPolls != null)
            {
                return Ok(previewPolls);
            }
            else
            {
                return StatusCode(500, new { status = "error", message = "An error occurred during the import process" });
            }
        }

        [HttpPost("polls/")]
        public async Task<IActionResult> SavePreviewPolls([FromBody] List<PollDTO> pollsInstances)
        {
            CreatedPollDTO createdPoll = await _cosmicLatteService.SavePreviewPolls(pollsInstances);
            if (createdPoll != null)
            {
                return Ok(createdPoll);
            }
            else
            {
                return StatusCode(500, new { status = "error", message = "An error occurred during the import process" });
            }
        }

        [HttpGet("polls/names")]
        public async Task<IActionResult> GetPollsNameList()
        {
            List<PollDataItem> pollList = await _cosmicLatteService.GetPollsNameList();
            return Ok(pollList);
        }

    }
}