using Microsoft.AspNetCore.Mvc;
using Eras.Application.Services;
using Eras.Application.DTOs;
using System.Threading.Tasks;
using MediatR;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Utils;
using Eras.Application.Contracts.Infrastructure;

[ApiController]
[Route("api/v1/[controller]")]
public class StudentsController : ControllerBase
{

    private readonly IMediator _mediator;
    private readonly ILogger<StudentsController> _logger;
    public StudentsController(IMediator mediator, ILogger<StudentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ImportStudents([FromBody] StudentImportDto[] students)
    {
        _logger.LogInformation("Importing {Count} students", students?.Length ?? 0);

        CreateStudentsCommand createStudentCommand = new CreateStudentsCommand() {students = students};
        BaseResponse response = await _mediator.Send(createStudentCommand);

        if (response.Success.Equals(true))
        {
            _logger.LogInformation("Successfully imported {Count} students", students?.Length ?? 0);
            return Ok(new { status = "successful", message = response.Message });
        }
        else
        {
            _logger.LogWarning("Failed to import students. Reason: {ResponseMessage}", response.Message);
            return StatusCode(400, new { status = "error", message = "An error occurred during the import process" });
        }
    }
}
