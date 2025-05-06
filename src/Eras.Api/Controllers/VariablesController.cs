using System.Diagnostics.CodeAnalysis;
using Eras.Application.Features.Variables.Queries.GetVariablesByPollUuidAndComponent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ExcludeFromCodeCoverage]
public class VariablesController(IMediator Mediator, ILogger<VariablesController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<VariablesController> _logger = Logger;

    [HttpGet]
    public async Task<IActionResult> GetAllByPollIdAndComponentsAsync(
        [FromQuery] string PollUuid,
        [FromQuery] List<string> Component
    )
    {
        List<Domain.Entities.Variable> result = await _mediator.Send(
            new GetVariablesByPollUuidAndComponentQuery(PollUuid, Component)
        );
        return Ok(result);
    }
}
