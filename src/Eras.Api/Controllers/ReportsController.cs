using MediatR;
using Microsoft.AspNetCore.Mvc;
using Eras.Application.Features.Consolidator.Queries.GetAvgRiskAnswer;

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
    public async Task<IActionResult> GetAvgRiskForStudents(List<int> studentIds, List<int> answerIds)
    {
        try
        {
            GetAvgRiskAnswerQuery query = new GetAvgRiskAnswerQuery() { StudentIds = studentIds, AnswerIds = answerIds };
            var avgRisk = await _mediator.Send(query);
            return Ok(new { status = "successful", message = avgRisk });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}
