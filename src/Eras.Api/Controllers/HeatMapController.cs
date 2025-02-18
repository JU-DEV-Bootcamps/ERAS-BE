using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Models;
using Eras.Application.Models.HeatMap;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class HeatMapController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HeatMapController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("components/polls/{pollId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHeatMapDataByAllComponents([FromRoute] string pollId)
        {
            BaseResponse response = await _mediator.Send(new GetHeatMapDataByAllComponentsQuery(pollId));
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
