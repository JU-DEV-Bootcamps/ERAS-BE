using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using Eras.Application.Features.Evaluations.Commands.UpdateEvaluation;
using Eras.Application.Features.Evaluations.Queries;
using Eras.Application.Features.Evaluations.Queries.GetAll;
using Eras.Application.Models;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EvaluationController> _logger;

        public EvaluationController(IMediator mediator, ILogger<EvaluationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluation(int id)
        {
            try
            {
                _logger.LogInformation("Deleting evaluation with ID {Id}", id);

                DeleteEvaluationCommand command = new DeleteEvaluationCommand() { id = id };

                BaseResponse response = await _mediator.Send(command);
                if (response.Success)
                {
                    _logger.LogInformation("Successfully deleted Evaluation {id}", id);
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
        public async Task<IActionResult> UpdateEvaluation(int id, [FromBody] EvaluationDTO evaluationDTO)
        {
            try
            {
                if (id != evaluationDTO.Id)
                {
                    _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match body ID {BodyId}", id, evaluationDTO.Id);
                    return BadRequest(new { status = "error", message = "ID mismatch between URL and body" });
                }
                _logger.LogInformation("Editing evaluation with ID {Id}", id);

                UpdateEvaluationCommand command = new UpdateEvaluationCommand() { EvaluationDTO = evaluationDTO };

                CreateCommandResponse<Evaluation> response = await _mediator.Send(command);
                if (response.Success)
                {
                    _logger.LogInformation("Successfully updated Evaluation {Name}", evaluationDTO.Name);
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
        public async Task<IActionResult> CreateEvaluation([FromBody] EvaluationDTO evaluationDTO)
        {
            _logger.LogInformation("Creating evaluation with the name {Name}", evaluationDTO.Name);
            CreateEvaluationCommand command = new CreateEvaluationCommand()
            {
                EvaluationDTO = evaluationDTO,
            };
            CreateCommandResponse<Evaluation> response = await _mediator.Send(command);
            if (response.Success)
            {
                _logger.LogInformation("Successfully created Evaluation {Name}", evaluationDTO.Name);
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

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEvaluationProcessSumary()
        {
            _logger.LogInformation("Getting evaluation process summary");
            GetEvaluationSummaryQuery summary = new();
            var res = await _mediator.Send(summary);
            return Ok(res);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEvaluations([FromQuery] Pagination query)
        {
            try
            {
                GetAllEvaluationsQuery command = new(query);

                var response = await _mediator.Send(command);
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
}
