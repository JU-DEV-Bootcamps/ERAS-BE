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
            try
            {
                var vm = await _mediator.Send(new GetHeatMapDataByAllComponentsQuery(pollId));
                var response = new GetQueryResponse<HeatMapByComponentsResponseVm>(vm, "Heatmap data requested successfully", true);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new GetQueryResponse<HeatMapByComponentsResponseVm>(null, "Failed Request, not found poll uuid", false));

            }
        }
    }
}
