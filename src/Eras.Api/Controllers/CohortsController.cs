using Eras.Application.Features.Cohort.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
            GetCohortsListQuery getCohortsListQuery = new();
            return Ok(await _mediator.Send(getCohortsListQuery));
        }

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCohortsSummary()
        {
            GetCohortsSummaryQuery getCohortsSummaryQuery = new();
            return Ok(await _mediator.Send(getCohortsSummaryQuery));
        }
    }
}