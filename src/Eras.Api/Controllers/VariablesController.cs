using System.Diagnostics.CodeAnalysis;
using Eras.Application.Features.Variables.Queries.GetVariablesByPollId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[ExcludeFromCodeCoverage]
public class VariablesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VariablesController> _logger;

    public VariablesController(IMediator mediator, ILogger<VariablesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByPollIdAndComponents(
        [FromQuery] string pollUuid,
        [FromQuery] List<string> component
    )
    {
        var result = await _mediator.Send(
            new GetVariablesByPollIdAndComponentQuery(pollUuid, component)
        );
        return Ok(result);
    }
}
