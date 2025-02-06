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
    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> ImportStudents([FromBody] StudentImportDto[] students)
    {

        CreateStudentsCommand createStudentCommand = new CreateStudentsCommand() {students = students};
        BaseResponse response = await _mediator.Send(createStudentCommand);

        if (response.Success)
        {
            return Ok(new { status = "successful", message = $"{success} Students imported successfully" });
        }
        else
        {
            return StatusCode(500, new { status = "error", message = "An error occurred during the import process" });
        }
    }
}
