using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.Variables.Queries.GetVariablesByPollUuidAndComponent;

using MediatR;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[ExcludeFromCodeCoverage]
public class VariablesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VariablesController> _logger;

    public VariablesController(IMediator Mediator, ILogger<VariablesController> Logger)
    {
        _mediator = Mediator;
        _logger = Logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByPollIdAndComponentsAsync(
        [FromQuery] string PollUuid,
        [FromQuery] List<string> Component
    )
    {
        var result = await _mediator.Send(
            new GetVariablesByPollUuidAndComponentQuery(PollUuid, Component)
        );
        return Ok(result);
    }
}
