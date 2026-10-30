using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Features.StudentInterventions;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

/// <summary>
/// Risks identified and support received for one student. Every user with an ERAS role reads them;
/// only an Administrator or a Professional changes them, and the handlers apply the ownership
/// (a Professional changes only their own entries) and the student scope of ADR-0004.
/// </summary>
[ApiController]
[Route("api/v1/students/{studentId:int}/interventions")]
[Authorize(Policy = ErasPolicies.AnyErasRole)]
public class StudentInterventionsController(IMediator Mediator) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentInterventionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStudentAsync([FromRoute] int studentId, CancellationToken cancellationToken)
    {
        IReadOnlyList<StudentInterventionDto> entries =
            await _mediator.Send(new GetStudentInterventionsQuery(studentId), cancellationToken);
        return Ok(entries);
    }

    [HttpPost]
    [Authorize(Policy = ErasPolicies.AdminOrProfessional)]
    [ProducesResponseType(typeof(StudentInterventionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAsync(
        [FromRoute] int studentId,
        [FromBody] SaveStudentInterventionDto entry,
        CancellationToken cancellationToken)
    {
        StudentInterventionDto created =
            await _mediator.Send(new CreateStudentInterventionCommand(studentId, entry), cancellationToken);
        return CreatedAtAction(nameof(GetByStudentAsync), new { studentId }, created);
    }

    [HttpPut("{entryId:int}")]
    [Authorize(Policy = ErasPolicies.AdminOrProfessional)]
    [ProducesResponseType(typeof(StudentInterventionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        [FromRoute] int studentId,
        [FromRoute] int entryId,
        [FromBody] SaveStudentInterventionDto entry,
        CancellationToken cancellationToken)
    {
        StudentInterventionDto updated =
            await _mediator.Send(new UpdateStudentInterventionCommand(studentId, entryId, entry), cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{entryId:int}")]
    [Authorize(Policy = ErasPolicies.AdminOrProfessional)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int studentId,
        [FromRoute] int entryId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteStudentInterventionCommand(studentId, entryId), cancellationToken);
        return NoContent();
    }
}
