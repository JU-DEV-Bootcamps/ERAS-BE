using Eras.Application.Dtos;
using Eras.Application.Features.Configurations.Queries.GetConfiguration;
using Eras.Application.Services;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/cosmic-latte")]
public class CosmicLatteController(IMediator Mediator, ICosmicLatteAPIService CosmicLatteService) : ControllerBase
{
    private readonly ICosmicLatteAPIService _cosmicLatteService = CosmicLatteService;
    private readonly IMediator _mediator = Mediator;

    [HttpGet("polls")]
    public async Task<IActionResult> GetPreviewPollsAsync(
        [FromQuery] string EvaluationSetName = "",
        [FromQuery] string StartDate = "",
        [FromQuery] string EndDate = "",
        [FromQuery] int ConfigurationId = 0
    )
    {
        GetConfigurationQuery getConfigurationQuery = new GetConfigurationQuery { ConfigurationId = ConfigurationId };
        var configuration = await _mediator.Send(getConfigurationQuery);
        return Ok(await _cosmicLatteService.GetAllPollsPreview(EvaluationSetName, StartDate, EndDate, configuration.EncryptedKey, configuration.BaseURL));
    }

    [HttpGet("health")]
    public async Task<IActionResult> IsCosmicLatteApiHealthyAsync([FromQuery] int ConfigurationId)
    {
        GetConfigurationQuery getConfigurationQuery = new GetConfigurationQuery { ConfigurationId = ConfigurationId };
        var configuration = await _mediator.Send(getConfigurationQuery);
        return Ok(await _cosmicLatteService.CosmicApiIsHealthy(configuration.EncryptedKey, configuration.BaseURL));
    }

    [Authorize]
    [HttpPost("polls")]
    public async Task<IActionResult> SavePreviewPollsAsync([FromBody] List<PollDTO> PollsInstances) => Ok(await _cosmicLatteService.SavePreviewPolls(PollsInstances));

    [HttpGet("polls/names")]
    public async Task<IActionResult> GetPollsNameListAsync([FromQuery] int ConfigurationId)
    {
        GetConfigurationQuery getConfigurationQuery = new GetConfigurationQuery { ConfigurationId = ConfigurationId };
        var configuration = await _mediator.Send(getConfigurationQuery);
        return Ok(await _cosmicLatteService.GetPollsNameList(configuration.BaseURL, configuration.EncryptedKey));
    }

}
