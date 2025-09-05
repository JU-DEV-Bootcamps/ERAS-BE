using Eras.Application.DTOs;
using Eras.Application.Features.Interventions.Commands.CreateIntervention;
using Eras.Application.Features.Interventions.Queries.GetInterventions;
using Eras.Application.Utils;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/interventions")]
[ExcludeFromCodeCoverage]
public class InterventionsController(IMediator Mediator, ILogger<InterventionsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<InterventionsController> _logger = Logger;
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetInterventionsAsync([FromQuery] Pagination Query)
    {
        return Ok(await _mediator.Send(new GetInterventionsQuery() { Query = Query }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateInterventionAsync([FromBody] JUInterventionDTO Intervention)
    {
        if (Intervention == null)
        {
            _logger.LogError("Intervention is null");
            return BadRequest("Intervention cannot be null");
        }
        CreateInterventionCommand createInterventionCommand = new CreateInterventionCommand()
        {
            Intervention = Intervention
        };
        var result = await _mediator.Send(createInterventionCommand);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Message);
    }
}