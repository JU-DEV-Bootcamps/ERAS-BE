using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.Polls.Queries.GetAllByPollAndCohort;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Application.Features.Polls.Queries.GetPollsByStudent;
using Eras.Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
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
        if(CohortId > 0 && StudentId > 0) return BadRequest("Only filter by StudentId or CohortId");
        if(CohortId == 0 && StudentId == 0) {
            return Ok(await _mediator.Send(new GetAllPollsQuery()));
        }
        if(StudentId > 0) {
            return Ok(await _mediator.Send(new GetPollsByStudentQuery() { StudentId = StudentId } ));
        }
            return Ok(await _mediator.Send(new GetPollsByCohortListQuery() { CohortId = CohortId }));
    }

    [HttpGet("{pollId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPollVariableByCohortAndPollAsync(
        [FromRoute] int PollId,
        [FromQuery] int CohortId
    ) => Ok(await _mediator.Send(new GetAllByPollAndCohortQuery(CohortId, PollId)));
}
