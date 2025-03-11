using Eras.Application.DTOs;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HeatMapController> _logger;

        public EvaluationController(IMediator mediator, ILogger<HeatMapController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvaluation([FromBody] EvaluationDTO evaluationDTO)
        {
            _logger.LogInformation("Creating evaluation with the name", evaluationDTO.Name);

           

           
                _logger.LogInformation("Successfully created Evaluaion ",evaluationDTO.Name);
                return Ok(new { status = "successful", message = "Created" });

           /*
                _logger.LogWarning(
                    "Failed to import students. Reason: {ResponseMessage}",
                    response.Message
                );
                return StatusCode(
                    400,
                    new { status = "error", message = "An error occurred during the import process" }
                );
            */
        }

    }
}
