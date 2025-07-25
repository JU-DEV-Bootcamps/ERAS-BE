﻿using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.Consolidator.Queries;
using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Application.Features.Consolidator.Queries.Students;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Eras.Application.Utils;
using Eras.Application.DTOs.Views;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
[ExcludeFromCodeCoverage]
public class ReportsController(IMediator Mediator) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;

    [HttpGet("students/top")]
    public async Task<IActionResult> GetHigherRiskStudentsByCohortAsync(
        [FromQuery] string CohortName,
        [FromQuery] string PollName,
        [FromQuery] int Take)
    {
        try
        {
            GetStudentTopQuery query = new() { CohortName = CohortName, PollName = PollName, Take = Take };
            GetQueryResponse<List<(Student Student, List<Answer> Answers, decimal RiskIndex)>> avgRisk = await _mediator.Send(query);
            var toprmessage = string.Join(", ", avgRisk.Body.Select(St => $"{St.Student.Uuid} - {St.Student.Name} - RISK = {St.RiskIndex}").ToList());
            var result = avgRisk.Body.Select(St => new
            {
                StudentUuid = St.Student.Uuid,
                StudentName = St.Student.Name,
                Answers = St.Answers?.Select((Ans) => new
                {
                    answer = Ans.AnswerText,
                    answerId = Ans.Id,
                    answerRiskLevel = Ans.RiskLevel,
                    variableId = Ans.PollVariableId,
                    pollInstanceId = Ans.PollInstanceId
                }).ToList(),
                St.RiskIndex
            }).ToList();

            return avgRisk.Success
            ? Ok(new
            {
                status = "successful",
                message = $"Top risk students: {toprmessage}",
                body = result
            })
            : BadRequest(new { status = "error", message = avgRisk.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { status = "error", message = ex.Message });
        }
    }


    [HttpGet("polls/{Uuid}/top")]
    public async Task<IActionResult> GetHigherRiskStudentsByPollAsync(
    [FromRoute] string Uuid,
    [FromQuery] Pagination Pagination,
    [FromQuery] string VariableIds)
    {
        try
        {
            List<int> VariableIdsAsInts = VariableIds.Split(',').Select(int.Parse).ToList();
            GetPollTopQuery query = new()
            {
                PollUuid = new Guid(Uuid),
                Pagination = Pagination,
                VariableIds = VariableIdsAsInts
            };
            PagedResult<ErasCalculationsByPollDTO>? avgRisk = await _mediator.Send(query);

            return
             Ok(new
             {
                 status = "successful",
                 message = "At-risk students retrieved sucessfully",
                 body = avgRisk
             });
        }
        catch (Exception ex)
        {
            return NotFound(new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("polls/{Uuid}/avg")]
    public async Task<IActionResult> GetAvgRiskByPollAsync([FromRoute] string Uuid, [FromQuery] string CohortIds, [FromQuery] bool LastVersion)
    {
        try
        {
            var pollGuid = new Guid(Uuid);
            List<int> CohortIdsAsInts = QueryParameterFilter.GetCohortIdsAsInts(CohortIds);

            if (CohortIdsAsInts.Count <= 0)
            {
                return BadRequest(new { status = "error", message = "Wrong format for cohortIds" });
            }
            var query = new PollAvgQuery() { PollUuid = pollGuid, CohortIds = CohortIdsAsInts, LastVersion = LastVersion };
            GetQueryResponse<AvgReportResponseVm> avgRisk = await _mediator.Send(query);
            return avgRisk.Success
            ? Ok(new
            {
                status = "successful",
                body = avgRisk.Body,
            })
            : BadRequest(new { status = "error", message = avgRisk.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("polls/{Uuid}/count")]
    public async Task<IActionResult> GetPollResultsCountAsync([FromRoute] string Uuid,
        [FromQuery] string CohortIds,
        [FromQuery] string VariableIds,
        [FromQuery] bool LastVersion)
    {
        try
        {
            var VariableIdsAsInts = VariableIds.Split(',').Select(int.Parse).ToList();
            var CohortIdsAsInts = QueryParameterFilter.GetCohortIdsAsInts(CohortIds);
            var query = new PollCountQuery() { PollUuid = Uuid, CohortIds = CohortIdsAsInts, VariableIds = VariableIdsAsInts, LastVersion = LastVersion };
            var count = await _mediator.Send(query);
            return count.Success
            ? Ok(new
            {
                status = "successful",
                body = count.Body,
            })
            : BadRequest(new { status = "error", message = count.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
    [HttpGet("polls/{Uuid}/risk-count")]
    public async Task<IActionResult> GetComponentSummaryByPollAsync(
    [FromRoute] string Uuid)
    {
        try
        {
            GetRiskCountQuery query = new()
            {
                PollUuid = new Guid(Uuid)
            };
            GetQueryResponse<RiskCountResponseVm> riskCountSummary = await _mediator.Send(query);
            return riskCountSummary.Success
            ? Ok(riskCountSummary)
            : NotFound(riskCountSummary);
        }
        catch (Exception ex)
        {
            return BadRequest(new { status = "error", message = ex.Message });
        }
    }
    [HttpGet("count")]
    public async Task<IActionResult> GetCountSummaryAsync()
    {
        GetCountSummaryQuery query = new();
        GetQueryResponse<Dictionary<string, int>> answer = await _mediator.Send(query);
        return Ok(answer);
    }
}
