using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class HeatMapController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<HeatMapController> _logger;

    public HeatMapController(IMediator mediator, ILogger<HeatMapController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("components/polls/{pollUUID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapDataByAllComponents([FromRoute] string pollUUID)
    {
        BaseResponse response = await _mediator.Send(
            new GetHeatMapDataByAllComponentsQuery(pollUUID)
        );
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("summary/polls/{pollUUID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummary([FromRoute] string pollUUID) 
    {
        BaseResponse response = await _mediator.Send(new GetHeatMapSummaryQuery(pollUUID));
        return response.Success ? Ok(response) : BadRequest(response);
    }
        
    [HttpGet("heatmap-details")]
    public async Task<IActionResult> GetStudentHeatMapDetailsByComponent(
        [FromQuery] string component
    )
    {
        var result = await _mediator.Send(new GetHeatMapDetailsByComponentQuery(component));
        return Ok(result);
    }
}
