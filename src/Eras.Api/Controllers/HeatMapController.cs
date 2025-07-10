using Eras.Application.DTOs.HeatMap;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapByPollIdAndVariableIds;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/heat-map")]
public class HeatMapController(IMediator Mediator, ILogger<HeatMapController> Logger)
    : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<HeatMapController> _logger = Logger;

    [HttpGet("polls/{Uuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapDataByAllComponentsAsync([FromRoute] string Uuid)
    {
        BaseResponse response = await _mediator.Send(
            new GetHeatMapDataByAllComponentsQuery(Uuid)
        );
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("polls/{Uuid}/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummaryAsync([FromRoute] string Uuid)
    {
        BaseResponse response = await _mediator.Send(new GetHeatMapSummaryQuery(Uuid));
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("cohorts/{Id}/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummaryByFiltersAsync(
    [FromRoute] int Id,
    [FromQuery] int Days
)
    {
        var getHeatMapSummaryByFiltersQuery = new GetHeatMapSummaryByFiltersQuery(Id, Days);
        GetQueryResponse<Application.Models.Response.HeatMap.HeatMapSummaryResponseVm> response =
            await _mediator.Send(getHeatMapSummaryByFiltersQuery);
        if (response.Success.Equals(true))
        {
            _logger.LogInformation(
                "Successfull request of heat map summary in {days} for cohort {cohortId}",
                Days,
                Id
            );
            return Ok(response);
        }
        else
        {
            _logger.LogWarning(
                "Failed to get heat map summary. Reason: {ResponseMessage}",
                response.Message
            );
            return BadRequest(response);
        }
    }


    [HttpGet("details")]
    public async Task<IActionResult> GetStudentHeatMapDetailsByComponentAsync(
        [FromQuery] string Component,
        [FromQuery] int Limit
    )
    {
        List<StudentHeatMapDetailDto> result =
            await _mediator.Send(new GetHeatMapDetailsByComponentQuery(Component, Limit));
        return Ok(result);
    }

    [HttpGet("cohorts/{Id}/top")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStudentHeatMapDetailsByCohortAsync(
        [FromRoute] string Id,
        [FromQuery] int Limit = 5
    )
    {
        List<StudentHeatMapDetailDto> result =
            await _mediator.Send(new GetHeatMapDetailsByCohortQuery(Id, Limit));
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapDataByPollUuidAndVariableIdsAsync(
        [FromBody] HeatMapBaseDataRequestDto Request
    )
    {
        List<Application.Models.Response.HeatMap.HeatMapBaseData> result = await _mediator.Send(
            new GetHeatMapByPollIdAndVariableIdsQuery(
                Request.pollInstanceUuid,
                Request.VariablesIds
            )
        );
        return Ok(result);
    }
}
