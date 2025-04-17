using Eras.Application.DTOs.HeatMap;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapByPollIdAndVariableIds;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters;
using Eras.Application.Models;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HeatMapController(IMediator Mediator, ILogger<HeatMapController> Logger)
    : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<HeatMapController> _logger = Logger;

    [HttpGet("components/polls/{pollUUID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapDataByAllComponentsAsync([FromRoute] string PollUUID)
    {
        BaseResponse response = await _mediator.Send(
            new GetHeatMapDataByAllComponentsQuery(PollUUID)
        );
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("summary/polls/{pollUUID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummaryAsync([FromRoute] string PollUUID)
    {
        BaseResponse response = await _mediator.Send(new GetHeatMapSummaryQuery(PollUUID));
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpGet("heatmap-details")]
    public async Task<IActionResult> GetStudentHeatMapDetailsByComponentAsync(
        [FromQuery] string Component,
        [FromQuery] int Limit
    )
    {
        List<Application.DTOs.HeatMap.StudentHeatMapDetailDto> result = await _mediator.Send(
            new GetHeatMapDetailsByComponentQuery(Component, Limit)
        );
        return Ok(result);
    }

    [HttpGet("cohort/{cohortId}/top-risk")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStudentHeatMapDetailsByCohortAsync(
        [FromRoute] string CohortId,
        [FromQuery] int Limit = 5
    )
    {
        List<Application.DTOs.HeatMap.StudentHeatMapDetailDto> result = await _mediator.Send(
            new GetHeatMapDetailsByCohortQuery(CohortId, Limit)
        );
        return Ok(result);
    }

    [HttpGet("summary/filter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummaryByFiltersAsync(
        [FromQuery] int CohortId,
        [FromQuery] int Days
    )
    {
        var getHeatMapSummaryByFiltersQuery = new GetHeatMapSummaryByFiltersQuery(CohortId, Days);
        GetQueryResponse<Application.Models.Response.HeatMap.HeatMapSummaryResponseVm> response = await _mediator.Send(getHeatMapSummaryByFiltersQuery);
        if (response.Success.Equals(true))
        {
            _logger.LogInformation(
                "Successfull request of heat map summary in {days} for cohort {cohortId}",
                Days,
                CohortId
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

    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapDataByPollUuidAndVariableIds(
        [FromBody] HeatMapBaseDataRequestDto request
    )
    {
        var result = await _mediator.Send(
            new GetHeatMapByPollIdAndVariableIdsQuery(
                request.pollInstanceUuid,
                request.VariablesIds
            )
        );
        return Ok(result);
    }
}
