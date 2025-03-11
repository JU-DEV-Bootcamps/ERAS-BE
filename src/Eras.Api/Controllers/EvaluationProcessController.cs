using MediatR;
using Microsoft.AspNetCore.Mvc;
using Eras.Application.Features.EvaluationProcess.Queries;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EvaluationProcessController(IMediator mediator, ILogger<EvaluationProcessController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<EvaluationProcessController> _logger = logger;

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEvaluationProcessSumary()
        {
            GetEvaluationProcessSummaryQuery summary = new();
            return Ok(await _mediator.Send(summary));
        }
    }
}
