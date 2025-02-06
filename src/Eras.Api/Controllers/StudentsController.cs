using Microsoft.AspNetCore.Mvc;
using Eras.Application.Services;
using Eras.Application.DTOs;
using System.Threading.Tasks;
using Eras.Application.Contracts.Infrastructure;

[ApiController]
[Route("api/v1/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost]
    public async Task<IActionResult> ImportStudents([FromBody] StudentImportDto[] students)
    {
        var success = await _studentService.ImportStudentsAsync(students);
        if (success>0)
        {
            return Ok(new { status = "successful", message = $"{success} Students imported successfully" });
        }
        else
        {
            return StatusCode(500, new { status = "error", message = "An error occurred during the import process" });
        }
    }
}
