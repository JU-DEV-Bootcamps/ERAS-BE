using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
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
            return Ok(await _cosmicLatteService.GetAllPollsPreview(name, startDate, endDate));
        }

        [HttpPost("polls/")]
        public async Task<IActionResult> SavePreviewPolls([FromBody] List<PollDTO> pollsInstances)
        {
            return Ok(await _cosmicLatteService.SavePreviewPolls(pollsInstances));
        }

        [HttpGet("polls/names")]
        public async Task<IActionResult> GetPollsNameList()
        {
            List<PollDataItem> pollList = await _cosmicLatteService.GetPollsNameList();
            return Ok(pollList);
        }

    }
}
