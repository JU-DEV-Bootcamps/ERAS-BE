using Eras.Application.Features.Answers.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnswersController(IMediator Mediator)
        {
            _mediator = Mediator;
        }

        [HttpGet("answers")]
        public async Task<IActionResult> GetStudentAnswersByPollAsync(
            [FromQuery] int StudentId,
            [FromQuery] int PollId
        )
        {
            GetStudentAnswersByPollQuery getStudentAnswersByPoll =
                new GetStudentAnswersByPollQuery() { StudentId = StudentId, PollId = PollId };
            return Ok(await _mediator.Send(getStudentAnswersByPoll));
        }
    }
}
