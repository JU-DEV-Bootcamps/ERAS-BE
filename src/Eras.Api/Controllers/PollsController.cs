using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.Polls.Queries.GetAllByPollAndCohort;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Application.Features.Polls.Queries.GetPollsByStudent;

using MediatR;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[ExcludeFromCodeCoverage]
public class PollsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PollsController> _logger;

    public PollsController(IMediator mediator, ILogger<PollsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPolls()
    {
        GetAllPollsQuery allPollsQuery = new GetAllPollsQuery();
        var result = await _mediator.Send(allPollsQuery);
        return Ok(result);
    }

    [HttpGet("cohort/{cohortId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPollsByCohort([FromRoute] int cohortId)
    {
        GetPollsByCohortListQuery getPollsByCohortListQuery = new GetPollsByCohortListQuery()
        {
            CohortId = cohortId,
        };
        return Ok(await _mediator.Send(getPollsByCohortListQuery));
    }

    [HttpGet("{pollId}/cohort/{cohortId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPollVariableByCohortAndPoll(
        [FromRoute] int pollId,
        [FromRoute] int cohortId
    )
    {
        var result = await _mediator.Send(new GetAllByPollAndCohortQuery(cohortId, pollId));
        return Ok(result);
    }

    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPollsByStudentId([FromRoute] int studentId)
    {
        GetPollsByStudentQuery getPollsByStudentQuery = new GetPollsByStudentQuery()
        {
            StudentId = studentId,
        };
        return Ok(await _mediator.Send(getPollsByStudentQuery));
    }
}
