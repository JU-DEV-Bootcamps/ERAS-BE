using System.ComponentModel;

using Eras.Application.Features.Cohorts.Queries;
using Eras.Application.Models.Response.Controllers.CohortsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eras.Api.Controllers;

[Description("Only for cohort level data. If Cohort is used only as filter use the students controller")]
[ApiController]
[Route("api/v1/cohorts")]
public class CohortsController(IMediator Mediator, ILogger<CohortsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<CohortsController> _logger = Logger;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCohortsAsync([FromQuery] string? PollUuid)
    {
        GetCohortsListQuery getCohortsListQuery = new();
        if (!string.IsNullOrEmpty(PollUuid))
        {
            getCohortsListQuery.PollUuid = PollUuid;
        }
        else
        {
            _logger.LogInformation("PollUuid is empty. Getting all cohorts");
        }
        Application.Models.Response.Common.GetQueryResponse<List<Cohort>> res = await _mediator.Send(getCohortsListQuery);
        return Ok(res);
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCohortsSummaryAsync(
        [FromQuery] Pagination Pagination
    )
    {
        GetCohortsSummaryQuery getCohortsSummaryQuery = new()
        {
            Pagination = Pagination
        };
        CohortSummaryResponse res = await _mediator.Send(getCohortsSummaryQuery);

        return Ok(res);
    }
    [HttpGet("details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCohortsDetailsAsync(
        [FromQuery] Pagination Pagination
    )
    {
        GetCohortsSummaryQuery getCohortsSummaryQuery = new()
        {
            Pagination = Pagination
        };
        CohortSummaryResponse res = await _mediator.Send(getCohortsSummaryQuery);

        return Ok(res);
    }
}
