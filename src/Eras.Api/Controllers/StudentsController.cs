using System.Diagnostics.CodeAnalysis;
using Eras.Application.DTOs;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll;
using Eras.Application.Features.Students.Queries.GetAllByPollAndDate;
using Eras.Application.Features.Students.Queries.GetStudentDetails;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
[ExcludeFromCodeCoverage]
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

        CreateStudentsCommand createStudentsCommand = new CreateStudentsCommand()
        {
            students = students,
        };
        CreateCommandResponse<Student[]> response = await _mediator.Send(createStudentsCommand);

        if (response.Success.Equals(true))
        {
            _logger.LogInformation("Successfully imported {Count} students", students?.Length ?? 0);
            return Ok(new { status = "successful", message = response.Message });
        }
        else
        {
            _logger.LogWarning(
                "Failed to import students. Reason: {ResponseMessage}",
                response.Message
            );
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

    [HttpGet("studentId")]
    public async Task<IActionResult> GetStudentDetailsById([FromQuery] int studentId)
    {
        GetStudentDetailsQuery getStudentDetailsQuery = new GetStudentDetailsQuery()
        {
            StudentDetailId = studentId
        };
        var result = await _mediator.Send(getStudentDetailsQuery);
        return Ok(result);
    }


    [HttpGet("poll/{pollUuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPreviewPolls(
        [FromQuery] Pagination query,
        [FromRoute] string pollUuid,
        [FromQuery] int days
    )
    {
        GetAllStudentsByPollUuidAndDaysQuery studentsByPollQuery =
            new GetAllStudentsByPollUuidAndDaysQuery()
            {
                Query = query,
                PollUuid = pollUuid,
                Days = days,
            };
        return Ok(await _mediator.Send(studentsByPollQuery));
    }

    [HttpGet("average/cohort/{cohortId}/poll/{pollId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAvgRiskByCohortAndPoll(
        [FromRoute] int cohortId,
        [FromRoute] int pollId
    )
    {
        var result = await _mediator.Send(
            new GetAllAverageRiskByCohortAndPollQuery(cohortId, pollId)
        );
        return Ok(result);
    }
}
