using Eras.Application.Features.Components.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ComponentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("RiskAvg")]
        public async Task<IActionResult> GetComponentsRiskAvgByStudent([FromQuery] int studentId, [FromQuery] int pollId)
        {
            GetComponentsAvgByStudentQuery getComponentsRiskAvgByStudent = new GetComponentsAvgByStudentQuery()
            {
                StudentId = studentId,
                PollId = pollId
            };
            return Ok(await _mediator.Send(getComponentsRiskAvgByStudent));
        }
    }
}
