using System.Diagnostics.CodeAnalysis;

using Eras.Application.DTOs;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Answers.Queries;
using Eras.Application.Features.Cohorts.Queries.GetCohortStudentsRiskByPoll;
using Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
using Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudentsByComponent;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Queries.GetAll;
using Eras.Application.Features.Students.Queries.GetAllAverageRiskByCohorAndPoll;
using Eras.Application.Features.Students.Queries.GetAllByPollAndDate;
using Eras.Application.Features.Students.Queries.GetStudentDetails;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/students")]
[Authorize]
[ExcludeFromCodeCoverage]
public class StudentsController(IMediator Mediator, ILogger<StudentsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<StudentsController> _logger = Logger;

    [HttpPost]
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
    public async Task<IActionResult> GetAllAsync([FromQuery] Pagination Query)
    {
        PagedResult<GetAllStudentsQueryResponse> result = await _mediator.Send(new GetAllStudentsQuery(Query));
        return Ok(result);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetStudentDetailsByIdAsync([FromRoute] int Id)
    {
        var getStudentDetailsQuery = new GetStudentDetailsQuery()
        {
            StudentDetailId = Id
        };
        CreateCommandResponse<Student> result = await _mediator.Send(getStudentDetailsQuery);
        return Ok(result);
    }


    [HttpGet("poll/{Uuid}")]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAvgRiskByCohortAndPollAsync(
        [FromQuery] string CohortIds,
        [FromQuery] string PollUuid,
        [FromQuery] Pagination Query,
        [FromQuery] bool LastVersion = true
    )
    {
        List<int> Ids = QueryParameterFilter.GetCohortIdsAsInts(CohortIds);
        PagedResult<StudentAverageRiskDto> result = await _mediator.Send(
            new GetAllAverageRiskByCohortAndPollQuery(Query, Ids, PollUuid, LastVersion)
        );
        return Ok(result);
    }

    //TODO: Implement views as: ?view=sum; ?view=top; ?view=avg as query params
    [HttpGet("polls/{Uuid}/sum")]
    public async Task<IActionResult> GetPollRiskSumStudentsAsync([FromRoute] string Uuid, [FromQuery] int CohortId)
    {
        var getCohortStudentsRiskByPollQuery = new GetCohortStudentsRiskByPollQuery()
        {
            PollUuid = Uuid,
            //Todo: Cohort Filter should be optional
            CohortId = CohortId
        };
        List<GetCohortStudentsRiskByPollResponse> queryResponse = await _mediator.Send(getCohortStudentsRiskByPollQuery);

        return Ok(queryResponse);
    }

    [HttpGet("polls/{Uuid}/top")]
    public async Task<IActionResult> GetPollTopStudentsAsync([FromRoute] string Uuid, [FromQuery] int CohortId, [FromQuery] bool LastVersion)
    {
        var getCohortTopRiskStudentsQuery = new GetCohortTopRiskStudentsQuery()
        {
            PollUuid = Uuid,
            //Todo: Cohort Filter should be optional
            CohortId = CohortId,
            LastVersion = LastVersion
        };
        GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>> queryResponse = await _mediator.Send(getCohortTopRiskStudentsQuery);

        return Ok(queryResponse.Body);
    }

    [HttpGet("polls/{Uuid}/components/top")]
    public async Task<IActionResult> GetComponentTopStudentsAsync([FromRoute] string Uuid, [FromQuery] string ComponentName, [FromQuery] int CohortId, [FromQuery] bool LastVersion)
    {
        var getCohortTopRiskStudentsByComponentQuery = new GetCohortTopRiskStudentsByComponentQuery()
        {
            PollUuid = Uuid,
            ComponentName = ComponentName,
            CohortId = CohortId,
            LastVersion = LastVersion,
        };
        GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>> queryResponse = await _mediator.Send(getCohortTopRiskStudentsByComponentQuery);

        return Ok(queryResponse.Body);
    }

    //TODO: Normalize use of uuid instead of id
    [HttpGet("{Id}/polls/{InstanceId}/answers")]
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
}
