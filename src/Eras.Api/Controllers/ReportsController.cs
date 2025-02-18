using MediatR;
using Microsoft.AspNetCore.Mvc;
using Eras.Application.Features.Consolidator.Queries.GetAvgRiskAnswer;
using Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;
using Eras.Application.Features.Consolidator.Queries.GetByRuleset;
namespace Eras.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAvgRiskStudents(List<int> studentIds, List<int> answerIds)
    {
        try
        {
            GetAvgRiskAnswerQuery query = new() { StudentIds = studentIds, AnswerIds = answerIds };
            var avgRisk = await _mediator.Send(query);
            return Ok(new { status = "successful", message = avgRisk });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetHigherRiskStudentsByCohort( string cohortId, int takeNumber)
    {
        try
        {
            GetHigherRiskStudentQuery query = new() { CohortId = cohortId, TakeNumber = takeNumber };
            var avgRisk = await _mediator.Send(query);
            return Ok(new { status = "successful", message = avgRisk });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetReportByRuleset(List<(int AnswerId, int Weight)> ruleset)
    {
        try
        {
            GetByRulesetQuery query = new() { RulesetVariablesWeight = ruleset };
            var report = await _mediator.Send(query);
            return Ok(new { status = "successful", message = report });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}
