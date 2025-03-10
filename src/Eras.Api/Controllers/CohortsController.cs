using System.Diagnostics.CodeAnalysis;
using Eras.Application.Features.Cohort.Queries.GetCohortsList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[ExcludeFromCodeCoverage]
public class CohortsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CohortsController> _logger;

    public CohortsController(IMediator mediator, ILogger<CohortsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCohorts()
    {
        GetCohortsListQuery getCohortsListQuery = new GetCohortsListQuery();
        return Ok(await _mediator.Send(getCohortsListQuery));
    }

}

