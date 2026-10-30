using System.Diagnostics.CodeAnalysis;

using Eras.Application.DTOs;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Answers.Queries;
using Eras.Application.Features.Cohorts.Queries.GetCohortStudentsRiskByPoll;
using Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
using Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudentsByComponent;
using Eras.Application.Features.Students.Commands.CreateManualStudent;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Commands.DeleteStudent;
using Eras.Application.Features.Students.Commands.UpdateStudentProfile;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohortAndPoll;
using Eras.Application.Features.Students.Queries.GetAllByPollAndDate;
using Eras.Application.Features.Students.Queries.GetAllLight;
using Eras.Application.Features.Students.Queries.GetStudentDetails;
using Eras.Application.Features.Students.Queries.GetStudentProfile;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/students")]
// Class policy is AnyErasRole; every action states its own policy (ADR-0004), a test enforces it.
[Authorize(Policy = ErasPolicies.AnyErasRole)]
public class StudentsController(IMediator Mediator, ILogger<StudentsController> Logger) : ControllerBase
{
    private const string GetStudentProfileRouteName = "GetStudentProfile";

    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<StudentsController> _logger = Logger;

    [HttpPost]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    public async Task<IActionResult> ImportStudentsAsync([FromBody] StudentImportDto[] Students)
    {
        _logger.LogInformation("Importing {Count} students", Students?.Length ?? 0);
        if (Students == null)
        {
            return BadRequest("No Students body found");
        }
        var createStudentsCommand = new CreateStudentsCommand()
        {
            students = Students,
        };
        CreateCommandResponse<Student[]> response = await _mediator.Send(createStudentsCommand);

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
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllAsync([FromQuery] Pagination Query, [FromQuery] string? Search = null)
    {
        PagedResult<GetAllStudentsQueryResponse> result = await _mediator.Send(new GetAllStudentsQuery(Query, Search));
        return Ok(result);
    }

    [HttpGet("{Id}")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    public async Task<IActionResult> GetStudentDetailsByIdAsync([FromRoute] int Id)
    {
        var getStudentDetailsQuery = new GetStudentDetailsQuery()
        {
            StudentId = Id
        };

        var result = await _mediator.Send(getStudentDetailsQuery);

        if (!result.Success || result.Entity == null)
        {
            return NotFound(new { status = "error", message = result.Message ?? "Student not found" });
        }

        return Ok(result);
    }


    [HttpGet("poll/{Uuid}")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPreviewPollsAsync(
        [FromQuery] Pagination Query,
        [FromRoute] string Uuid,
        [FromQuery] int Days
    )
    {
        var studentsByPollQuery =
            new GetAllStudentsByPollUuidAndDaysQuery()
            {
                Query = Query,
                PollUuid = Uuid,
                Days = Days,
            };
        return Ok(await _mediator.Send(studentsByPollQuery));
    }

    [HttpGet("average")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAvgRiskByCohortAndPollAsync(
        [FromQuery] string CohortIds,
        [FromQuery] string PollUuid,
        [FromQuery] Pagination Query,
        [FromQuery] bool LastVersion,
        [FromQuery] int? evaluationId
    )
    {
        List<int> Ids = QueryParameterFilter.GetCohortIdsAsInts(CohortIds);
        PagedResult<StudentAverageRiskDto> result = await _mediator.Send(
            new GetAllAverageRiskByCohortAndPollQuery(Query, Ids, PollUuid, LastVersion, evaluationId)
        );
        return Ok(result);
    }

    [HttpGet("polls/{Uuid}/sum")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    public async Task<IActionResult> GetPollRiskSumStudentsAsync([FromRoute] string Uuid, [FromQuery] int CohortId)
    {
        var getCohortStudentsRiskByPollQuery = new GetCohortStudentsRiskByPollQuery()
        {
            PollUuid = Uuid,
            CohortId = CohortId
        };
        List<GetCohortStudentsRiskByPollResponse> queryResponse = await _mediator.Send(getCohortStudentsRiskByPollQuery);

        return Ok(queryResponse);
    }

    [HttpGet("polls/{Uuid}/top")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    public async Task<IActionResult> GetPollTopStudentsAsync(
        [FromRoute] string Uuid, [FromQuery] int CohortId, [FromQuery] bool LastVersion, [FromQuery] Pagination Query)
    {
        var getCohortTopRiskStudentsQuery = new GetCohortTopRiskStudentsQuery()
        {
            PollUuid = Uuid,
            CohortId = CohortId,
            LastVersion = LastVersion,
            PageValues = Query
        };
        GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>> queryResponse = await _mediator.Send(getCohortTopRiskStudentsQuery);

        return Ok(queryResponse.Body);
    }

    [HttpGet("polls/{Uuid}/components/top")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    public async Task<IActionResult> GetComponentTopStudentsAsync(
        [FromRoute] string Uuid, [FromQuery] string ComponentName, [FromQuery] int CohortId, [FromQuery] bool LastVersion, [FromQuery] Pagination Query)
    {
        var getCohortTopRiskStudentsByComponentQuery = new GetCohortTopRiskStudentsByComponentQuery()
        {
            PollUuid = Uuid,
            ComponentName = ComponentName,
            CohortId = CohortId,
            LastVersion = LastVersion,
            PageValues = Query
        };
        GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>> queryResponse = await _mediator.Send(getCohortTopRiskStudentsByComponentQuery);

        return Ok(queryResponse.Body);
    }

    [HttpGet("{Id}/polls/{InstanceId}/answers")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    public async Task<IActionResult> GetStudentAnswersByPollAsync(
        [FromRoute] int Id,
        [FromRoute] int InstanceId,
        [FromQuery] Pagination Query
    )
    {
        var getStudentAnswersByPoll =
            new GetStudentAnswersByPollQuery() { StudentId = Id, PollId = InstanceId, Query = Query };
        try
        {
            return Ok(await _mediator.Send(getStudentAnswersByPoll));
        }
        catch (Exception e)
        {
            _logger.LogWarning("Could not get answers for that {StudentId} of pollInstance={PollInstanceId}.\n {e}", Id, InstanceId, e);
            return NotFound($"Could not get answers for that {Id} of pollInstance={InstanceId}.\n {e}");
        }
    }

    [HttpGet("light")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    public async Task<IActionResult> GetAllLightAsync()
    {
        var result = await _mediator.Send(new GetAllStudentsLightQuery());
        return Ok(result);
    }

    [HttpPost("manual")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(typeof(StudentRegistrationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateManualStudentAsync([FromBody] StudentRegistrationDto Profile)
    {
        StudentRegistrationDto created = await _mediator.Send(new CreateManualStudentCommand(Profile));
        return CreatedAtRoute(GetStudentProfileRouteName, new { Id = created.StudentId }, created);
    }

    [HttpGet("{Id:int}/profile", Name = GetStudentProfileRouteName)]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(StudentRegistrationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentProfileAsync([FromRoute] int Id)
    {
        StudentRegistrationDto profile = await _mediator.Send(new GetStudentProfileQuery(Id));
        return Ok(profile);
    }

    [HttpPut("{Id:int}/profile")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(typeof(StudentRegistrationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStudentProfileAsync(
        [FromRoute] int Id,
        [FromBody] StudentRegistrationDto Profile)
    {
        StudentRegistrationDto updated = await _mediator.Send(new UpdateStudentProfileCommand(Id, Profile));
        return Ok(updated);
    }

    [HttpDelete("{Id:int}")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteStudentAsync([FromRoute] int Id)
    {
        await _mediator.Send(new DeleteStudentCommand(Id));
        return NoContent();
    }
}
