using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters;
using Eras.Application.Models;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HeatMapController(IMediator Mediator, ILogger<HeatMapController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator ?? throw new ArgumentNullException(nameof(Mediator));
    private readonly ILogger<HeatMapController> _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));

    [HttpGet("polls/{pollUUID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapDataByAllComponentsAsync([FromRoute] string PollUUID)
    {
        if (string.IsNullOrWhiteSpace(PollUUID))
        {
            _logger.LogWarning("Invalid pollUUID provided.");
            return BadRequest("pollUUID cannot be null or empty.");
        }

        try
        {
            _logger.LogInformation("Fetching heat map data for pollUUID: {pollUUID}", PollUUID);
            GetQueryResponse<IEnumerable<Application.Models.HeatMap.HeatMapByComponentsResponseVm>> response = await _mediator.Send(new GetHeatMapDataByAllComponentsQuery(PollUUID));
            return response.Success ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching heat map data for pollUUID: {pollUUID}", PollUUID);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpGet("polls/{pollUUID}/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummaryAsync([FromRoute] string PollUUID)
    {
        if (string.IsNullOrWhiteSpace(PollUUID))
        {
            _logger.LogWarning("Invalid pollUUID provided.");
            return BadRequest("pollUUID cannot be null or empty.");
        }

        try
        {
            _logger.LogInformation("Fetching heat map summary for PollUUID: {PollUUID}", PollUUID);
            GetQueryResponse<Application.Models.HeatMap.HeatMapSummaryResponseVm> response = await _mediator.Send(new GetHeatMapSummaryQuery(PollUUID));
            return response.Success ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching heat map summary for pollUUID: {pollUUID}", PollUUID);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpGet("details")]
    public async Task<IActionResult> GetStudentHeatMapDetailsByComponentAsync(
        [FromQuery] string Component,
        [FromQuery] int Limit)
    {
        if (string.IsNullOrWhiteSpace(Component))
        {
            _logger.LogWarning("Invalid component provided.");
            return BadRequest("Component cannot be null or empty.");
        }

        try
        {
            _logger.LogInformation("Fetching heat map details for component: {component} with limit: {limit}", Component, Limit);
            List<Application.DTOs.HeatMap.StudentHeatMapDetailDto> result = await _mediator.Send(new GetHeatMapDetailsByComponentQuery(Component, Limit));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching heat map details for component: {component}", Component);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpGet("cohort/{cohortId}/top")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStudentHeatMapDetailsByCohortAsync(
        [FromRoute] string CohortId,
        [FromQuery] int Limit = 5)
    {
        if (string.IsNullOrWhiteSpace(CohortId))
        {
            _logger.LogWarning("Invalid cohortId provided.");
            return BadRequest("cohortId cannot be null or empty.");
        }

        try
        {
            _logger.LogInformation("Fetching top-risk heat map details for cohortId: {cohortId} with limit: {limit}", CohortId, Limit);
            List<Application.DTOs.HeatMap.StudentHeatMapDetailDto> result = await _mediator.Send(new GetHeatMapDetailsByCohortQuery(CohortId, Limit));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching top-risk heat map details for cohortId: {cohortId}", CohortId);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpGet("summary/filter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHeatMapSummaryByFiltersAsync([FromQuery] int CohortId, [FromQuery] int Days)
    {
        if (CohortId <= 0 || Days <= 0)
        {
            _logger.LogWarning("Invalid cohortId or days provided.");
            return BadRequest("cohortId and days must be greater than zero.");
        }

        try
        {
            _logger.LogInformation("Fetching heat map summary for cohortId: {cohortId} and days: {days}", CohortId, Days);
            var query = new GetHeatMapSummaryByFiltersQuery(CohortId, Days);
            GetQueryResponse<Application.Models.HeatMap.HeatMapSummaryResponseVm> response = await _mediator.Send(query);

            if (response.Success)
            {
                _logger.LogInformation("Successfully fetched heat map summary for cohortId: {cohortId} and days: {days}", CohortId, Days);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("Failed to fetch heat map summary. Reason: {ResponseMessage}", response.Message);
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching heat map summary for cohortId: {cohortId} and days: {days}", CohortId, Days);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }
}
