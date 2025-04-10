using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ExcludeFromCodeCoverage]
    public class PollInstanceController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<StudentsController> _logger;
        public PollInstanceController(IMediator mediator, ILogger<StudentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPollInstancesBy(
        [FromQuery] string lastDays = ""
        )
        {

            _logger.LogInformation("Getting poll instances in the last {days}", lastDays);

            GetPollInstancesByLastDaysQuery createStudentCommand = new GetPollInstancesByLastDaysQuery() { LastDays = int.Parse(lastDays) };
            QueryManyResponse<PollInstance> response = await _mediator.Send(createStudentCommand);

            if (response.Success.Equals(true))
            {
                _logger.LogInformation("Successfull request of poll instances in {days}", lastDays);
                return Ok(new { status = response.Success, message = response.Message, body = response.Entities });
            }
            else
            {
                _logger.LogWarning("Failed to get poll instances. Reason: {ResponseMessage}", response.Message);
                return StatusCode(400, new { status = "error", message = "An error occurred during the get process" });
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetPollInstancesByCohortIdAndDays([FromQuery] int cohortId, [FromQuery] int days)
        {
            _logger.LogInformation("Getting poll instances in the last {days} for cohort {cohortId}", days, cohortId);
            var getPollInstancesByCohortIdAndDaysQuery = new GetPollInstanceByCohortAndDaysQuery(cohortId, days);
            var response = await _mediator.Send(getPollInstancesByCohortIdAndDaysQuery);
            if (response.Success.Equals(true))
            {
                _logger.LogInformation("Successfull request of poll instances in {days} for cohort {cohortId}", days, cohortId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Failed to get poll instances. Reason: {ResponseMessage}", response.Message);
                return StatusCode(400, response);
            }
        }

    }
}
