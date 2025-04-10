using System.Diagnostics.CodeAnalysis;

using Eras.Application.DTOs;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll;
using Eras.Application.Features.Students.Queries.GetAllByPollAndDate;
using Eras.Application.Features.Students.Queries.GetStudentDetails;
using Eras.Application.Models;
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

    public StudentsController(IMediator Mediator, ILogger<StudentsController> Logger)
    {
        _mediator = Mediator;
        _logger = Logger;
    }

    [HttpPost]
    public async Task<IActionResult> ImportStudentsAsync([FromBody] StudentImportDto[] Students)
    {
        _logger.LogInformation("Importing {Count} students", Students?.Length ?? 0);

        CreateStudentsCommand createStudentCommand = new CreateStudentsCommand()
        {
            students = Students ?? [],
        };
        CreateCommandResponse<Student[]> response = await _mediator.Send(createStudentCommand);

        if (response.Success.Equals(true))
        {
            _logger.LogInformation("Successfully imported {Count} students", Students?.Length ?? 0);
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
    public async Task<IActionResult> GetAllAsync([FromQuery] Pagination Query)
    {
        var result = await _mediator.Send(new GetAllStudentsQuery(Query));
        return Ok(result);
    }

    [HttpGet("studentId")]
    public async Task<IActionResult> GetStudentDetailsByIdAsync([FromQuery] int StudentId)
    {
        GetStudentDetailsQuery getStudentDetailsQuery = new GetStudentDetailsQuery()
        {
            StudentDetailId = StudentId
        };
        var result = await _mediator.Send(getStudentDetailsQuery);
        return Ok(result);
    }


    [HttpGet("poll/{pollUuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPreviewPollsAsync(
        [FromQuery] Pagination Query,
        [FromRoute] string PollUuid,
        [FromQuery] int Days
    )
    {
        GetAllStudentsByPollUuidAndDaysQuery studentsByPollQuery =
            new GetAllStudentsByPollUuidAndDaysQuery()
            {
                Query = Query,
                PollUuid = PollUuid,
                Days = Days,
            };
        return Ok(await _mediator.Send(studentsByPollQuery));
    }

    [HttpGet("average/cohort/{cohortId}/poll/{pollId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAvgRiskByCohortAndPollAsync(
        [FromRoute] int CohortId,
        [FromRoute] int PollId
    )
    {
        var result = await _mediator.Send(
            new GetAllAverageRiskByCohortAndPollQuery(CohortId, PollId)
        );
        return Ok(result);
    }
}
