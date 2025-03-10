﻿using Eras.Application.Features.Cohort.Queries;
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
                CohortId = s.Student.Cohort.Id,
                CohortName = s.Student.Cohort.Name,
                PollinstancesAverage = s.PollInstances.Average(p => p.Answers.Average(a => a.RiskLevel)),
                PollinstancesCount = s.PollInstances.Count,
            }).ToList();
            return Ok(result);
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
                CohortName = s.Student.Cohort.Name,
                s.Student.Cohort,
                PollinstancesAverage = s.PollInstances.Average(p => p.Answers.Average(a => a.RiskLevel)),
                Pollinstances = s.PollInstances
            }).ToList();
            return Ok(result);
        }
    }
}
