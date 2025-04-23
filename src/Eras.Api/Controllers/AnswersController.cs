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

        public AnswersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("answers")]
        public async Task<IActionResult> GetStudentAnswersByPoll(
            [FromQuery] int studentId,
            [FromQuery] int pollId
        )
        {
            GetStudentAnswersByPollQuery getStudentAnswersByPoll =
                new GetStudentAnswersByPollQuery() { StudentId = studentId, PollId = pollId };
            return Ok(await _mediator.Send(getStudentAnswersByPoll));
        }
    }
}
