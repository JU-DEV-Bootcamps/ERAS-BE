﻿using Eras.Application.Features.Cohort.Queries;
using Eras.Application.Features.Cohort.Queries.GetCohortComponentsByPoll;
using Eras.Application.Features.Cohort.Queries.GetCohortStudentsRiskByPoll;
using Eras.Application.Features.Cohort.Queries.GetCohortTopRiskStudents;
using Eras.Application.Features.Cohort.Queries.GetCohortTopRiskStudentsByComponent;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CohortsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CohortsController> _logger;

        public CohortsController(IMediator mediator, ILogger<CohortsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCohorts()
        {
            GetCohortsListQuery getCohortsListQuery = new();
            return Ok(await _mediator.Send(getCohortsListQuery));
        }

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCohortsSummary()
        {
            GetCohortsSummaryQuery getCohortsSummaryQuery = new();
            var queryRes = await _mediator.Send(getCohortsSummaryQuery);

            var result = queryRes.Select(s => new
            {
                StudentUuid = s.Student.Uuid,
                StudentName = s.Student.Name,
                CohortId = s.Student.Cohort?.Id,
                CohortName = s.Student.Cohort?.Name,
                PollinstancesAverage = s.PollInstances.Average(p => p.Answers.Average(a => a.RiskLevel)),
                PollinstancesCount = s.PollInstances.Count,
            }).ToList();
            return Ok(new {
                CohortCount = result.Select(s => s.CohortName).Distinct().Count(),
                StudentCount = result.Count,
                Summary = result
            });
        }
        [HttpGet("details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCohortsDetails()
        {
            GetCohortsSummaryQuery getCohortsSummaryQuery = new();
            var queryRes = await _mediator.Send(getCohortsSummaryQuery);

            var result = queryRes.Select(s => new
            {
                StudentUuid = s.Student.Uuid,
                StudentName = s.Student.Name,
                s.Student,
                CohortName = s.Student.Cohort?.Name,
                s.Student.Cohort,
                PollinstancesAverage = s.PollInstances.Average(p => p.Answers.Average(a => a.RiskLevel)),
                Pollinstances = s.PollInstances
            }).ToList();
            return Ok(new
            {
                CohortCount = result.Select(s => s.CohortName).Distinct().Count(),
                StudentCount = result.Count,
                Summary = result
            });
        }

        [HttpGet("componentsAvg")]
        public async Task<IActionResult> GetCohortsComponentsAvg([FromQuery] string PollUuid)
        {
            GetCohortComponentsByPollQuery getCohortComponentsByPollQuery = new GetCohortComponentsByPollQuery() { PollUuid = PollUuid};
            var queryResponse = await _mediator.Send(getCohortComponentsByPollQuery);

            var mappedResponse = queryResponse
                                .GroupBy(x => new { x.CohortId, x.CohortName })
                                .Select(group => new
                                {
                                    CohortId = group.Key.CohortId,
                                    CohortName = group.Key.CohortName,
                                    ComponentsAvg = group.ToDictionary(
                                        g => g.ComponentName,
                                        g => g.AverageRiskByCohortComponent
                                    )
                                })
                                .ToList();

            return Ok(mappedResponse);
        }

        [HttpGet("studentsRisk")]
        public async Task<IActionResult> GetCohortStudentsRiskByPoll([FromQuery] string PollUuid, [FromQuery] int CohortId)
        {
            GetCohortStudentsRiskByPollQuery getCohortStudentsRiskByPollQuery = new GetCohortStudentsRiskByPollQuery()
            {
                PollUuid = PollUuid,
                CohortId = CohortId
            };
            var queryResponse = await _mediator.Send(getCohortStudentsRiskByPollQuery);

            return Ok(queryResponse);
        }

        [HttpGet("studentsRiskByComponent")]
        public async Task<IActionResult> GetCohortTopStudentsByComponent([FromQuery] string PollUuid, [FromQuery] string ComponentName, [FromQuery] int CohortId)
        {
            GetCohortTopRiskStudentsByComponentQuery getCohortTopRiskStudentsByComponentQuery = new GetCohortTopRiskStudentsByComponentQuery()
            {
                PollUuid = PollUuid,
                ComponentName = ComponentName,
                CohortId = CohortId,
            };
            var queryResponse = await _mediator.Send(getCohortTopRiskStudentsByComponentQuery);

            return Ok(queryResponse);
        }

        [HttpGet("studentsRiskList")]
        public async Task<IActionResult> GetCohortTopStudents([FromQuery] string PollUuid, [FromQuery] int CohortId)
        {
            GetCohortTopRiskStudentsQuery getCohortTopRiskStudentsQuery = new GetCohortTopRiskStudentsQuery()
            {
                PollUuid = PollUuid,
                CohortId = CohortId,
            };
            var queryResponse = await _mediator.Send(getCohortTopRiskStudentsQuery);

            return Ok(queryResponse);
        }

    }
}
