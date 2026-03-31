using System.Diagnostics.CodeAnalysis;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers.AssessmentManagement;

[ApiController]
[Route("api/v1/assessments")]
[Authorize]
[ExcludeFromCodeCoverage]
public class AssessmentsController(IMediator Mediator, ILogger<AssessmentsController> Logger) : ControllerBase
{

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AssessmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssessmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetRemissionByIdQuery(id), cancellationToken);

        return response is null
            ? NotFound()
            : Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetAllRemissionsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("by-student/{studentId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetByStudentId(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetRemissionsByStudentIdQuery(studentId), cancellationToken);
        if (response.Any())
        {
            return Ok(response);
        }
        return NotFound();
    }

    [HttpGet("by-status/{status}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetByStatus(
        string status,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AssessmentStatus>(status, true, out var parsedStatus))
        {
            return BadRequest($"Invalid remission status '{status}'.");
        }

        var response = await Mediator.Send(new GetRemissionsByStatusQuery(parsedStatus), cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AssessmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssessmentDto>> Create(
        [FromBody] AssessmentDto dto,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new CreateRemissionCommand(dto), cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AssessmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssessmentDto>> Update(
        Guid id,
        [FromBody] AssessmentDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.Id.HasValue && dto.Id.Value != id)
        {
            return BadRequest("Route id must match payload id.");
        }

        var request = dto with { Id = id };

        var response = await Mediator.Send(new UpdateRemissionCommand(request), cancellationToken);

        return Ok(response);
    }
}
