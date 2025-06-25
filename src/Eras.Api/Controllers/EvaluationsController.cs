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

namespace Eras.Api.Controllers;

[Route("api/v1/evaluations")]
[ApiController]
[Authorize]
public class EvaluationsController(IMediator Mediator, ILogger<EvaluationsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<EvaluationsController> _logger = Logger;

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvaluationAsync(int Id)
    {
        try
        {
            _logger.LogInformation("Deleting evaluation with ID {Id}", Id);

            var command = new DeleteEvaluationCommand() { id = Id };

            BaseResponse response = await _mediator.Send(command);
            if (response.Success)
            {
                _logger.LogInformation("Successfully deleted Evaluation {id}", Id);
                return Ok(new { status = "successful", message = "Deleted" });
            }
            else
            {
                _logger.LogError("Failed to delete Evaluation. Reason: {ResponseMessage}", response.Message);
                return StatusCode(
                    500,
                    new { status = "error", message = "An error occurred during the evaluation update process" }
                );
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Failed to get information. Reason: {ResponseMessage}",
                ex.Message
);
            return StatusCode(
                500,
                new { status = "error", message = "An error occurred during deletion operation" }
            );

        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvaluationAsync(int Id, [FromBody] EvaluationDTO EvaluationDTO)
    {
        try
        {
            if (Id != EvaluationDTO.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match body ID {BodyId}", Id, EvaluationDTO.Id);
                return BadRequest(new { status = "error", message = "ID mismatch between URL and body" });
            }
            _logger.LogInformation("Editing evaluation with ID {Id}", Id);

            var command = new UpdateEvaluationCommand() { EvaluationDTO = EvaluationDTO };

            CreateCommandResponse<Evaluation> response = await _mediator.Send(command);
            if (response.Success)
            {
                _logger.LogInformation("Successfully updated Evaluation {Name}", EvaluationDTO.Name);
                return Ok(new { status = "successful", message = "Updated" });
            }
            else
            {
                _logger.LogError("Failed to update Evaluation. Reason: {ResponseMessage}", response.Message);
                return StatusCode(
                    400,
                    new { status = "error", message = "An error occurred during the evaluation update process" }
                );
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Failed to get information. Reason: {ResponseMessage}",
                ex.Message
);
            return StatusCode(
                500,
                new { status = "error", message = "An error occurred during the information access" }
            );

        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvaluationAsync([FromBody] EvaluationDTO EvaluationDTO)
    {
        _logger.LogInformation("Creating evaluation with the name {Name}", EvaluationDTO.Name);
        var command = new CreateEvaluationCommand()
        {
            EvaluationDTO = EvaluationDTO,
        };
        CreateCommandResponse<Evaluation> response = await _mediator.Send(command);
        if (response.Success)
        {
            _logger.LogInformation("Successfully created Evaluation {Name}", EvaluationDTO.Name);
            return Ok(new { status = "successful", message = "Created" });
        }
        else
        {
            _logger.LogError(
                "Failed to create Evaluation. Reason: {ResponseMessage}",
                response.Message
);
            return StatusCode(
                400,
                new { status = "error", message = "An error occurred during the evaluation creation process" }
            );
        }
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
        try
        {
            GetAllEvaluationsQuery command = new(Query);
            PagedResult<Evaluation> response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Failed to get information. Reason: {ResponseMessage}",
                ex.Message
            );
            return StatusCode(
                500,
                new { status = "error", message = "An error occurred during the information access" }
            );
        }
    }




}
