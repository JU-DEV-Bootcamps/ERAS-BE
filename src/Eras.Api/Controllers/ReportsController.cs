using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Application.Features.Consolidator.Queries.Students;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/reports")]
[ExcludeFromCodeCoverage]
public class ReportsController(IMediator Mediator) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;

    [HttpGet("students/avg")]
    public async Task<IActionResult> GetAvgRiskStudentsAsync(
        [FromQuery] string PollInstanceUuid,
        [FromQuery] int? CohortId)
    {
        try
        {
            var pollGuid = new Guid(PollInstanceUuid);
            var query = new PollAvgQuery() { PollUuid = pollGuid, CohortId = CohortId ?? 0 };
            GetQueryResponse<Application.Models.Consolidator.AvgReportResponseVm> avgRisk = await _mediator.Send(query);
            return avgRisk.Success
            ? Ok(new
            {
                status = "successful",
                body = avgRisk
            })
            : BadRequest(new { status = "error", message = avgRisk.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("students/top")]
    public async Task<IActionResult> GetHigherRiskStudentsByCohortAsync(
        [FromQuery] string CohortName,
        [FromQuery] string PollName,
        [FromQuery] int Take)
    {
        try
        {
            GetStudentTopQuery query = new() { CohortName = CohortName, PollName = PollName, Take = Take };
            GetQueryResponse<List<(Student Student, List<Answer> Answers, double RiskIndex)>> avgRisk = await _mediator.Send(query);
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
    [FromQuery] int Take,
    [FromQuery] string VariableIds)
    {
        try
        {
            GetPollTopQuery query = new()
            {
                PollUuid = new Guid(Uuid),
                Take = Take,
                VariableIds = VariableIds
            };
            GetQueryResponse<List<(Answer answer, Variable variable, Student student)>> avgRisk = await _mediator.Send(query);
            var topRiskMessage = string.Join(", ", avgRisk.Body.Select(Stud =>
                $"{Stud.student.Uuid} - {Stud.student.Name} - RISK = {Stud.answer.RiskLevel}"
            ).ToList());
            var result = avgRisk.Body.Select(Stud => new
            {
                Stud.student,
                Stud.variable,
                Stud.answer,
            }).ToList();

            return avgRisk.Success
            ? Ok(new
            {
                status = "successful",
                message = $"Top risk students: {topRiskMessage}",
                body = result
            })
            : BadRequest(new { status = "error", message = avgRisk.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { status = "error", message = ex.Message });
        }
    }

    [HttpGet("polls/{Uuid}/avg")]
    public async Task<IActionResult> GetAvgRiskByPollAsync([FromRoute] string Uuid, [FromQuery] int CohortId)
    {
        try
        {
            var pollGuid = new Guid(Uuid);
            var query = new PollAvgQuery() { PollUuid = pollGuid, CohortId = CohortId };
            GetQueryResponse<Application.Models.Consolidator.AvgReportResponseVm> avgRisk = await _mediator.Send(query);
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
    [HttpGet("polls/{Uuid}/summary")]
    public async Task<IActionResult> GetComponentSummaryByPollAsync(
    [FromRoute] string Uuid)
    {
        try
        {
            GetComponentSummaryQuery query = new()
            {
                PollUuid = new Guid(Uuid)
            };
            GetQueryResponse<List<Answer>> answers = await _mediator.Send(query);
            var risks = answers.Body.Select(Answer => Answer.RiskLevel);
            var averageRisk = risks.Any() ? risks.Average() : 0;

            return answers.Success
            ? Ok(new
            {
                status = "successful",
                message = $"Poll Variable summary:",
                body = new
                {
                    risks = risks,
                    averageRisk = averageRisk
                }
            })
            : BadRequest(new { status = "error", message = "Success"});
        }
        catch (Exception ex)
        {
            return NotFound(new { status = "error", message = ex.Message });
        }
    }
}
