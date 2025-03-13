using Eras.Application.DTOs;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Queries;
using Eras.Application.Models;
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

        [HttpPost]
        public async Task<IActionResult> CreateEvaluation([FromBody] EvaluationDTO evaluationDTO)
        {
            _logger.LogInformation("Creating evaluation with the name {Name}", evaluationDTO.Name);
            CreateEvaluationCommand command = new CreateEvaluationCommand()
            {
                EvaluationDTO = evaluationDTO,
            };
            CreateComandResponse<Evaluation> response = await _mediator.Send(command);
            if (response.Success)
            {
                _logger.LogInformation("Successfully created Evaluation {Name}", evaluationDTO.Name);
                return Ok(new { status = "successful", message = "Created" });
            } else
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
    }
}
