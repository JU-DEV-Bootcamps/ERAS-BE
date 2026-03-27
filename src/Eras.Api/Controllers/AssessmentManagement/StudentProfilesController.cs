using System.Diagnostics.CodeAnalysis;

using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers.AssessmentManagement;

[ApiController]
[Route("api/v1/student-profiles")]
[Authorize]
[ExcludeFromCodeCoverage]
public class StudentProfilesController(IMediator Mediator, ILogger<StudentProfilesController> Logger) : ControllerBase
{

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentProfileDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetStudentProfileByIdQuery(id), cancellationToken);

        return response is null
            ? NotFound()
            : Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<StudentProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<StudentProfileDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetAllStudentProfilesQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("by-student-code/{studentCode}")]
    [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentProfileDto>> GetByStudentCode(
        string studentCode,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetStudentProfileByStudentCodeQuery(studentCode), cancellationToken);

        return response is null
            ? NotFound()
            : Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentProfileDto>> Create(
        [FromBody] StudentProfileDto dto,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new CreateStudentProfileCommand(dto), cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentProfileDto>> Update(
        Guid id,
        [FromBody] StudentProfileDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.Id.HasValue && dto.Id.Value != id)
        {
            return BadRequest("Route id must match payload id.");
        }

        var request = dto with { Id = id };

        var response = await Mediator.Send(new UpdateStudentProfileCommand(request), cancellationToken);

        return Ok(response);
    }
}
