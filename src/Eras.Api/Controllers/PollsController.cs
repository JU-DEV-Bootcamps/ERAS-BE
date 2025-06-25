using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.Polls.Queries.GetAllByPollAndCohort;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Application.Features.Polls.Queries.GetPollsByStudent;
using Eras.Application.Features.Variables.Queries.GetVariablesByPollUuidAndComponent;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/polls")]
[Authorize]
[ExcludeFromCodeCoverage]
public class PollsController(IMediator Mediator, ILogger<PollsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<PollsController> _logger = Logger;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPollsByCohortAsync([FromQuery] int CohortId, [FromQuery] int StudentId)
    {
        if (CohortId > 0 && StudentId > 0) return BadRequest("Only filter by StudentId or CohortId");
        if (CohortId == 0 && StudentId == 0)
        {
            return Ok(await _mediator.Send(new GetAllPollsQuery()));
        }
        if (StudentId > 0)
        {
            return Ok(await _mediator.Send(new GetPollsByStudentQuery() { StudentId = StudentId }));
        }
        return Ok(await _mediator.Send(new GetPollsByCohortListQuery() { CohortId = CohortId }));
    }

    [HttpGet("{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPollVariableByCohortAndPollAsync(
        [FromRoute] int Id,
        [FromQuery] int CohortId
    ) => Ok(await _mediator.Send(new GetAllByPollAndCohortQuery(CohortId, Id)));

    [HttpGet("{Uuid}/variables")]
    public async Task<IActionResult> GetVariablesByComponentsAsync(
        [FromRoute] string Uuid,
        [FromQuery] List<string> Component,
        [FromQuery] bool LastVersion
    )
    {
        List<Variable> result = await _mediator.Send(
            new GetVariablesByPollUuidAndComponentQuery(Uuid, Component, LastVersion)
        );
        return Ok(result);
    }
}
