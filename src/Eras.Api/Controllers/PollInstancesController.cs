using System.Diagnostics.CodeAnalysis;

using Eras.Application.Utils;
using Eras.Application.Features.Cohorts.Queries.GetCohortComponentsByPoll;
using Eras.Application.Features.Components.Queries;
using Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[Route("api/v1/poll-instances")]
[ApiController]
[ExcludeFromCodeCoverage]
public class PollInstancesController(IMediator Mediator, ILogger<StudentsController> Logger) : ControllerBase
{

    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<StudentsController> _logger = Logger;

    [HttpGet]
    public async Task<IActionResult> GetPollInstancesByCohortIdAndDaysAsync(
            [FromQuery] int[] CohortId,
            [FromQuery] int Days,
            [FromQuery] Pagination Query
    )
    {
        return Ok(await _mediator.Send(new GetPollInstanceByCohortAndDaysQuery(Query, CohortId, Days)));
    }

    [HttpGet("{Uuid}/cohorts/avg")]
    public async Task<IActionResult> GetComponentsAvgGroupedByCohortAsync([FromRoute] string Uuid)
    {
        var getCohortComponentsByPollQuery = new GetCohortComponentsByPollQuery() { PollUuid = Uuid };
        List<Application.Models.Response.Calculations.GetCohortComponentsByPollResponse> queryResponse = await _mediator.Send(getCohortComponentsByPollQuery);
        var mappedResponse = queryResponse
            .GroupBy(X => new { X.CohortId, X.CohortName })
            .Select(Group => new
            {
                Group.Key.CohortId,
                Group.Key.CohortName,
                ComponentsAvg = Group.ToDictionary(
                    G => G.ComponentName,
                    G => G.AverageRiskByCohortComponent
                )
            })
            .ToList();
        return Ok(mappedResponse);
    }

    [HttpGet("{Id}/avg")]
    public async Task<IActionResult> GetComponentsRiskAvgByStudentAsync([FromQuery] int StudentId, [FromRoute] int Id)
    {
        var getComponentsRiskAvgByStudent = new GetComponentsAvgByStudentQuery()
        {
            StudentId = StudentId,
            PollId = Id
        };
        return Ok(await _mediator.Send(getComponentsRiskAvgByStudent));
    }
}
