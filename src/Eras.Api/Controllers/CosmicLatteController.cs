using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Services;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Eras.Api.Controllers
{
    [ApiController]
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
