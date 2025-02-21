using Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
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

            _logger.LogInformation("Getting poll instances in the last {days} students", lastDays);

            GetPollInstancesByLastDaysQuery createStudentCommand = new GetPollInstancesByLastDaysQuery() { LastDays = int.Parse(lastDays)};
            QueryManyResponse<PollInstance> response = await _mediator.Send(createStudentCommand);

            if (response.Success.Equals(true))
            {
                _logger.LogInformation("Successfull request of poll instances in {days}", lastDays);
                return Ok(new { status = response.Success, message = response.Message, body = response.Entities });
            }
            else
            {
                _logger.LogWarning("Failed to import students. Reason: {ResponseMessage}", response.Message);
                return StatusCode(400, new { status = "error", message = "An error occurred during the import process" });
            }
        }
    }
}
