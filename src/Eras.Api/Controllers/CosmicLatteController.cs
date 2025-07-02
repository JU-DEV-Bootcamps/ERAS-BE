using Eras.Application.Dtos;
using Eras.Application.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/cosmic-latte")]
public class CosmicLatteController(ICosmicLatteAPIService CosmicLatteService) : ControllerBase
{
    private readonly ICosmicLatteAPIService _cosmicLatteService = CosmicLatteService;

    [HttpGet("polls")]
    public async Task<IActionResult> GetPreviewPollsAsync(
    [FromQuery] string EvaluationSetName = "",
    [FromQuery] string StartDate = "",
    [FromQuery] string EndDate = ""
    ) => Ok(await _cosmicLatteService.GetAllPollsPreview(EvaluationSetName, StartDate, EndDate));

    [Authorize]
    [HttpPost("polls")]
    public async Task<IActionResult> SavePreviewPollsAsync([FromBody] List<PollDTO> PollsInstances) => Ok(await _cosmicLatteService.SavePreviewPolls(PollsInstances));

    [HttpGet("polls/names")]
    public async Task<IActionResult> GetPollsNameListAsync() => Ok(await _cosmicLatteService.GetPollsNameList());

}
