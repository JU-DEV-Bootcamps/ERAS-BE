using Microsoft.AspNetCore.Mvc;
using Eras.Application.Services;
using Eras.Application.DTOs;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentService _studentService;

    public StudentsController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost]
    public async Task<IActionResult> ImportStudents([FromBody] StudentImportDto[] students)
    {
        var success = await _studentService.ImportStudentsAsync(students);
        if (success)
        {
            return Ok(new { status = "successful", message = "Students imported successfully" });
        }
        else
        {
            return StatusCode(500, new { status = "error", message = "An error occurred during the import process" });
        }
    }
}
