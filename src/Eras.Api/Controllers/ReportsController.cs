using MediatR;
using Microsoft.AspNetCore.Mvc;
using Eras.Application.Features.Consolidator.Queries.GetAvgRiskAnswer;
using Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;
using Eras.Application.Features.Consolidator.Queries.GetByRuleset;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
namespace Eras.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ExcludeFromCodeCoverage]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions SerializeOptions = new() { IncludeFields = true };

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("avgrisk/students/")]
    public async Task<IActionResult> GetAvgRiskStudents([FromBody] List<List<int>> studentIdsanswerIds)
    {
        List<int> studentIds = studentIdsanswerIds[0];
        List<int> answerIds = studentIdsanswerIds[1];
        try
        {
            //TODO: Domain Implementation pending
            GetAvgRiskAnswerQuery query = new() { StudentIds = studentIds, AnswerIds = answerIds };
            var avgRisk = await _mediator.Send(query);
            return StatusCode(501, new { status = "not implemented", message = "Implementation to get avgRisk Pending" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("higherrisk/byCohortPoll/")]
    public async Task<IActionResult> GetHigherRiskStudentsByCohort(
        [FromQuery] string cohortName,
        [FromQuery] string pollName,
        [FromQuery] int take)
    {
        try
        {
            GetHigherRiskStudentByCohortPollQuery query = new() { CohortName = cohortName, PollName = pollName, Take = take };
            var avgRisk = await _mediator.Send(query);
            var toprmessage = string.Join(", ", avgRisk.Body.Select(s => $"{s.Student.Uuid} - {s.Student.Name} - RISK = {s.RiskIndex}").ToList());
            var result = avgRisk.Body.Select(s => new
            {
                StudentUuid = s.Student.Uuid,
                StudentName = s.Student.Name,
                Answers = s.Answers?.Select((a, i)=> new {
                    answer = a.AnswerText,
                    answerId = a.Id,
                    answerRiskLevel = a.RiskLevel,
                    variableId = a.PollVariableId,
                    pollInstanceId = a.PollInstanceId
                    }).ToList(),
                s.RiskIndex
            }).ToList();

            return avgRisk.Success
            ? Ok(new {
                    status = "successful",
                    message = $"Top risk students: {toprmessage}",
                    body = result
                })
            : BadRequest(new { status = "error", message = avgRisk.Message });}
        catch (Exception ex)
        {
            return NotFound(new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("higherrisk/byVariable/")]
    public async Task<IActionResult> GetHigherRiskStudentsByVariable(
        [FromQuery] int variableId,
        [FromQuery] string pollInstanceUuid,
        [FromQuery] int take)
    {
        try
        {
            GetHigherRiskStudentByVariableQuery query = new() { VariableId = variableId, PollInstanceUuid = pollInstanceUuid, Take = take };
            var avgRisk = await _mediator.Send(query);
            var toprmessage = string.Join(", ", avgRisk.Body.Select(s => $"{s.student.Uuid} - {s.student.Name} - RISK = {s.answer.RiskLevel}").ToList());
            var result = avgRisk.Body.Select(s => new
            {
                s.student,
                s.variable,
                s.answer,
            }).ToList();

            return avgRisk.Success
            ? Ok(new {
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


    [HttpGet("byruleset/")]
    public async Task<IActionResult> GetReportByRuleset([FromBody] List<(int AnswerId, int Weight)> ruleset)
    {
        try
        {
            //TODO: Domain Implementation pending
            GetByRulesetQuery query = new() { RulesetVariablesWeight = ruleset };
            var report = await _mediator.Send(query);
            return StatusCode(501, new { status = "not implemented", message = "Implementation to get by ruleset Pending" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }



}
