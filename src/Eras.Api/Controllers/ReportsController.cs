using MediatR;
using Microsoft.AspNetCore.Mvc;
using Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;
using System.Diagnostics.CodeAnalysis;
using Eras.Application.Features.Consolidator.Queries;
namespace Eras.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ExcludeFromCodeCoverage]
public class ReportsController(IMediator Mediator) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;

    [HttpGet("students/avg")]
    public async Task<IActionResult> GetAvgRiskStudentsAsync([FromQuery] string PollInstanceUuid)

    {
        try
        {
            var pollGuid = new Guid(PollInstanceUuid);
            var query = new PollAvgQuery() { PollUuid = pollGuid };
            Application.Models.BaseResponse avgRisk = await _mediator.Send(query);
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
            GetHigherRiskStudentByCohortPollQuery query = new() { CohortName = CohortName, PollName = PollName, Take = Take };
            var avgRisk = await _mediator.Send(query);
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


    [HttpGet("higherrisk/byPoll/")]
    public async Task<IActionResult> GetHigherRiskStudentsByPollAsync(
    [FromQuery] string PollInstanceUuid,
    [FromQuery] int Take,
    [FromQuery] string VariableIds)
    {
        try
        {
            PollTopQuery query = new() {PollUuid = new Guid(PollInstanceUuid)};
            Application.Models.BaseResponse avgRisk = await _mediator.Send(query);
            // var topRiskMessage = string.Join(", ", avgRisk.Body.Select(Student =>
            //     $"{Student.student.Uuid} - {Student.student.Name} - RISK = {Student.answer.RiskLevel}"
            // ).ToList());
            // var result = avgRisk.Body.Select(Students => new
            // {
            //     Students.student,
            //     Students.variable,
            //     Students.answer,
            // }).ToList();

            return avgRisk.Success
            ? Ok(new
            {
                status = "successful",
                body = avgRisk.Message
            })
            : BadRequest(new { status = "error", message = avgRisk.Message });
        }
        catch (Exception ex)
        {
            return NotFound(new { status = "error", message = ex.Message });
        }
    }
}
