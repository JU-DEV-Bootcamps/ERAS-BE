using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Eras.Application.Features.Polls.Queries.GetPollsByStudent;

[ApiController]
[Route("api/v1/[controller]")]
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
        GetPollsByCohortListQuery getPollsByCohortListQuery = new GetPollsByCohortListQuery() { CohortId = cohortId };
        return Ok(await _mediator.Send(getPollsByCohortListQuery));
    }

    [HttpGet("student/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPollsByStudentId([FromRoute] int studentId)
    {
        GetPollsByStudentQuery getPollsByStudentQuery = new GetPollsByStudentQuery() { StudentId = studentId };
        return Ok(await _mediator.Send(getPollsByStudentQuery));
    }

}
