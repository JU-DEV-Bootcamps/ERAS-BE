using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using Eras.Application.Features.Evaluations.Commands.UpdateEvaluation;
using Eras.Application.Features.Evaluations.Queries;
using Eras.Application.Features.Evaluations.Queries.GetAll;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Eras.Application.Features.Evaluations.Queries.GetByDateRange;

namespace Eras.Api.Controllers;

[Route("api/v1/evaluations")]
[ApiController]
public class EvaluationsController(IMediator Mediator, ILogger<EvaluationsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<EvaluationsController> _logger = Logger;

    [HttpDelete("{Id}")]
    [Authorize]
    public async Task<IActionResult> DeleteEvaluationAsync(int Id)
    {
        _logger.LogInformation("Deleting evaluation with ID {Id}", Id);
        var command = new DeleteEvaluationCommand() { id = Id };
        BaseResponse response = await _mediator.Send(command);

        _logger.LogInformation("Successfully deleted Evaluation {id}", Id);
        return Ok(new { status = "successful", message = "Deleted" });
    }

    [Authorize]
    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateEvaluationAsync(int Id, [FromBody] EvaluationDTO EvaluationDTO)
    {
        if (Id != EvaluationDTO.Id)
        {
            _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match body ID {BodyId}", Id, EvaluationDTO.Id);
            return BadRequest(new { status = "error", message = "ID mismatch between URL and body" });
        }

        _logger.LogInformation("Editing evaluation with ID {Id}", Id);
        var command = new UpdateEvaluationCommand() { EvaluationDTO = EvaluationDTO };
        CreateCommandResponse<Evaluation> response = await _mediator.Send(command);

        _logger.LogInformation("Successfully updated Evaluation {Name}", EvaluationDTO.Name);
        return Ok(new { status = "successful", message = "Updated" });
    }

    [Authorize]
    [HttpPost("{ParentId}")]
    public async Task<IActionResult> CreateEvaluationAsync(string ParentId, [FromBody] EvaluationDTO EvaluationDTO)
    {
        _logger.LogInformation("Creating evaluation with the name {Name}", EvaluationDTO.Name);
        var command = new CreateEvaluationCommand()
        {
            EvaluationDTO = EvaluationDTO,
            ParentId = ParentId
        };
        CreateCommandResponse<Evaluation> response = await _mediator.Send(command);

        _logger.LogInformation("Successfully created Evaluation {Name}", EvaluationDTO.Name);
        return Ok(new { status = "successful", message = "Created" });
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetEvaluationDetailsAsync(int Id)
    {
        _logger.LogInformation("Getting evaluation process summary");
        GetEvaluationSummaryQuery summary = new() { EvaluationId = Id };
        var res = await _mediator.Send(summary);
        return res.Body == null ? NotFound(res) : Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEvaluationsAsync([FromQuery] Pagination Query)
    {
        GetAllEvaluationsQuery command = Query != null ? new() : new() { Query = Query };
        PagedResult<Evaluation> response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("ByDateRange")]
    public async Task<IActionResult> GetAllEvaluationsByDateRangeAsync([FromQuery] DateTime StartDate, [FromQuery] DateTime EndDate)
    {
        GetEvaluationsByDateRangeQuery query = new GetEvaluationsByDateRangeQuery
        {
            StartDate = StartDate,
            EndDate = EndDate
        };
        List<Evaluation> response = await _mediator.Send(query);
        return Ok(response);
    }
}
