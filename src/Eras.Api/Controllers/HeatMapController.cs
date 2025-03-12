using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters;

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
        [FromQuery] string component,
        [FromQuery] int limit
    )
    {
        var result = await _mediator.Send(new GetHeatMapDetailsByComponentQuery(component, limit));
        return Ok(result);
    }

    [HttpGet("cohort/{cohortId}/top-risk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStudentHeatMapDetailsByCohort(
    [FromRoute] string cohortId,
    [FromQuery] int limit = 5)
    {
        var result = await _mediator.Send(new GetHeatMapDetailsByCohortQuery(cohortId, limit));
        return Ok(result);
    }

    [HttpGet("summary/filter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummaryByFilters([FromQuery] int cohortId, [FromQuery] int days)
    {
        var getHeatMapSummaryByFiltersQuery = new GetHeatMapSummaryByFiltersQuery(cohortId, days);
        var response = await _mediator.Send(getHeatMapSummaryByFiltersQuery);
        if (response.Success.Equals(true))
        {
            _logger.LogInformation("Successfull request of heat map summary in {days} for cohort {cohortId}", days, cohortId);
            return Ok(response);
        }
        else
        {
            _logger.LogWarning("Failed to get heat map summary. Reason: {ResponseMessage}", response.Message);
            return BadRequest(response);
        }
    }

}
