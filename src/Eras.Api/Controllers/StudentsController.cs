using System.Threading.Tasks;
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Services;
using Eras.Application.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        CreateStudentsCommand createStudentCommand = new CreateStudentsCommand()
        {
            students = students,
        };
        BaseResponse response = await _mediator.Send(createStudentCommand);

        if (response.Success.Equals(true))
        {
            return Ok(new { status = "successful", message = response.Message });
        }
        else
        {
            return StatusCode(
                400,
                new { status = "error", message = "An error occurred during the import process" }
            );
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Pagination query)
    {
        var result = await _mediator.Send(new GetAllStudentsQuery(query));
        return Ok(result);
    }
}
