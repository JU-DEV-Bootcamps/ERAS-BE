using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetPolls()
        {
            return Ok("All list of polls in our DB");
        }

        [HttpGet("cohort/{cohortId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPollsByCohort([FromRoute] int cohortId)
        {
            GetPollsByCohortListQuery getPollsByCohortListQuery = new GetPollsByCohortListQuery() { CohortId = cohortId };
            return Ok(await _mediator.Send(getPollsByCohortListQuery));
        }

    }
