using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PollsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PollsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPolls()
        {
            GetAllPollsQuery allPollsQuery = new GetAllPollsQuery();
            var result = await _mediator.Send(allPollsQuery);
            return Ok(result);
        }
    }
}
