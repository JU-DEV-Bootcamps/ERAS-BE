using System.ComponentModel;

using Eras.Application.Features.Cohort.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

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
            _logger.LogError("PollUuid is empty. Getting all cohorts");
        }
        return Ok(await _mediator.Send(getCohortsListQuery));
    }

    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCohortsSummaryAsync()
    {
        GetCohortsSummaryQuery getCohortsSummaryQuery = new();
        List<(Domain.Entities.Student Student, List<Domain.Entities.PollInstance> PollInstances)> queryRes = await _mediator.Send(getCohortsSummaryQuery);

        var result = queryRes.Select(S => new
        {
            StudentUuid = S.Student.Uuid,
            StudentName = S.Student.Name,
            CohortId = S.Student.Cohort?.Id,
            CohortName = S.Student.Cohort?.Name,
            PollinstancesAverage = S.PollInstances.Average(P => P.Answers.Average(A => A.RiskLevel)),
            PollinstancesCount = S.PollInstances.Count,
        }).ToList();
        return Ok(new
        {
            CohortCount = result.Select(S => S.CohortName).Distinct().Count(),
            StudentCount = result.Count,
            Summary = result
        });
    }
    [HttpGet("details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCohortsDetailsAsync()
    {
        GetCohortsSummaryQuery getCohortsSummaryQuery = new();
        List<(Domain.Entities.Student Student, List<Domain.Entities.PollInstance> PollInstances)> queryRes = await _mediator.Send(getCohortsSummaryQuery);

        var result = queryRes.Select(S => new
        {
            StudentUuid = S.Student.Uuid,
            StudentName = S.Student.Name,
            S.Student,
            CohortName = S.Student.Cohort?.Name,
            S.Student.Cohort,
            PollinstancesAverage = S.PollInstances.Average(P => P.Answers.Average(A => A.RiskLevel)),
            Pollinstances = S.PollInstances
        }).ToList();
        return Ok(new
        {
            CohortCount = result.Select(S => S.CohortName).Distinct().Count(),
            StudentCount = result.Count,
            Summary = result
        });
    }
}
