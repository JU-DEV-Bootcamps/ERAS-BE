using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Models;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Eras.Api.Controllers.AssessmentManagement;

/// <summary>
/// Endpoints under interventions/attachments are gated with AnyErasRole (role-only)
/// rather than restricted to the resource's own creator/assigned professional:
/// ownership-scoped enforcement is tracked separately (#545).
/// </summary>
[ApiController]
[Route("api/v1/assessments")]
[Authorize]
public class AssessmentsController(
    IMediator Mediator,
    IFileStorageService FileStorage,
    ICurrentUserService CurrentUser) : ControllerBase
{
    private readonly IFileStorageService _fileStorage = FileStorage;
    private readonly ICurrentUserService _currentUser = CurrentUser;

    [HttpGet("{id:int}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(AssessmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssessmentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetRemissionByIdQuery(id), cancellationToken);

        return response is null
            ? NotFound()
            : Ok(response);
    }

    [HttpGet]
    [Authorize(Policy = ErasPolicies.AdminOnly)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetAllRemissionsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("by-student/{studentId:int}")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetByStudentId(
        int studentId,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetRemissionsByStudentIdQuery(studentId), cancellationToken);

        return response.Any()
            ? Ok(response)
            : NotFound();
    }

    [HttpGet("by-status/{status}")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetByStatus(
        string status,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AssessmentStatus>(status, true, out var parsedStatus))
            return BadRequest($"Invalid remission status '{status}'.");

        var response = await Mediator.Send(new GetRemissionsByStatusQuery(parsedStatus), cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(typeof(AssessmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssessmentDto>> Create(
        [FromBody] AssessmentDto dto,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new CreateRemissionCommand(dto), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(typeof(AssessmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssessmentDto>> Update(
        int id,
        [FromBody] AssessmentDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.Id == default)
            return BadRequest("Route id must match payload id.");

        try
        {
            var response = await Mediator.Send(new UpdateRemissionCommand(dto with { Id = id }), cancellationToken);
            return Ok(response);
        }
        catch (OperationCanceledException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = ErasPolicies.AdminOrOfficer)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        try {
            await Mediator.Send(new DeleteAssessmentCommand(id), cancellationToken);
            return NoContent();
        } catch(KeyNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpGet("{id:int}/interventions")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(IReadOnlyCollection<InterventionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<InterventionDto>>> GetInterventions(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetInterventionsByAssessmentQuery(id), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Interventions of the assessment, scoped to a Student Services Officer: only returned
    /// when <paramref name="creatorSub"/> is the assessment's own creator (#545).
    /// </summary>
    [HttpGet("{id:int}/interventions/by-creator/{creatorSub}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(IReadOnlyCollection<InterventionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<InterventionDto>>> GetInterventionsByCreator(
        int id,
        string creatorSub,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetInterventionsByAssessmentAndCreatorQuery(id, creatorSub), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Interventions of the assessment scoped to a Professional: those created by
    /// <paramref name="professionalSub"/> or assigned to them, as long as the caller is
    /// the same professional (#545).
    /// </summary>
    [HttpGet("{id:int}/interventions/by-professional/{professionalSub}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(IReadOnlyCollection<InterventionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<InterventionDto>>> GetInterventionsByAssignedProfessional(
        int id,
        string professionalSub,
        CancellationToken cancellationToken)
    {
        if (_currentUser.Sub != professionalSub)
            return Forbid();

        var response = await Mediator.Send(
            new GetInterventionsByAssessmentAndAssignedProfessionalQuery(id, professionalSub, _currentUser.Name),
            cancellationToken);
        return Ok(response);
    }

    [HttpPost("interventions")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(InterventionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InterventionDto>> AddIntervention(
        [FromBody] AddInterventionDto dto,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(
            new AddInterventionCommand(dto.AssessmentId, dto.Intervention, dto.DraftSessionId), cancellationToken);
        return Created(string.Empty, response);
    }

    [HttpPut("{id:int}/interventions")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(IReadOnlyCollection<InterventionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<InterventionDto>>> UpsertInterventions(
        int id,
        [FromBody] IReadOnlyCollection<InterventionDto> interventions,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await Mediator.Send(new UpsertInterventionsCommand(id, interventions), cancellationToken);
            return Ok(response);
        }
        catch (OperationCanceledException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}/interventions/{interventionId:int}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIntervention(
        int id,
        int interventionId,
        CancellationToken cancellationToken)
    {
        try
        {
            await Mediator.Send(new DeleteInterventionCommand(id, interventionId), cancellationToken);
            return NoContent();
        } catch(KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("interventions/{interventionId}/attachments")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyCollection<string>>> UploadAttachments(
        int interventionId,
        [FromForm] IFormFileCollection files,
        CancellationToken cancellationToken)
    {
        if (files.Count == 0)
            return BadRequest("No files provided.");

        var fileStreams = files
            .Select(f => (f.OpenReadStream(), f.FileName))
            .ToList();

        try
        {
            var result = await Mediator.Send(
                new UploadInterventionAttachmentsCommand(interventionId, fileStreams),
                cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { title = ex.Message });
        }
    }

    [HttpGet("interventions/{interventionId}/attachments/{fileName}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachment(
        int interventionId,
        string fileName,
        CancellationToken cancellationToken)
    {
        try
        {
            string relativePath = Path.Combine("interventions", interventionId.ToString(), fileName)
                .Replace('\\', '/');

            Stream stream = await _fileStorage.ReadAsync(relativePath);
            string contentType = GetContentType(fileName);

            return File(stream, contentType, enableRangeProcessing: false);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    private static string GetContentType(string fileName) =>
        Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };


    [HttpDelete("interventions/{interventionId:int}/attachments/{fileName}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttachment(
        int interventionId,
        string fileName,
        CancellationToken cancellationToken)
    {
        try
        {
            await Mediator.Send(
                new DeleteInterventionAttachmentCommand(interventionId, fileName),
                cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{assessmentId:int}/interventions/{interventionId:int}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(InterventionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateInterventionAsync(
        int assessmentId, int interventionId,
        [FromBody] UpdateInterventionRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await Mediator.Send(
                new UpdateInterventionCommand(assessmentId, interventionId, request.UpdateInterventionDto, request.AttachmentIdsToRemove, request.DraftSessionId));
            return Ok(response);
        }
        catch (KeyNotFoundException) 
        {
            return NotFound();
        }
        catch (OperationCanceledException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }


    [HttpPut("{assessmentId:int}/interventions/{interventionId:int}/replace-type")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(InterventionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ReplaceInterventionType(
        int assessmentId, int interventionId,
        [FromBody] UpdateInterventionRequestDto Request,
        CancellationToken CancellationToken)
    {
        try
        {
            var command = new ReplaceInterventionCommand(
                assessmentId,
                interventionId,
                Request.UpdateInterventionDto,
                Request.AttachmentIdsToRemove,
                Request.DraftSessionId);

            UpdateInterventionDto result = await Mediator.Send(command, CancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException) 
        {
            return NotFound();
        }
    }

    [HttpGet("by-creator/{creatorSub}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetByCreatorSub(
        string creatorSub,
        CancellationToken cancellationToken)
    {
        IEnumerable<AssessmentDto> response =
            await Mediator.Send(new GetAssessmentsByCreatorQuery(creatorSub), cancellationToken);

        return Ok(response);
    }

    [HttpGet("by-professional/{professionalSub}")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetByProfessionalSub(
        string professionalSub,
        CancellationToken cancellationToken)
    {
        IEnumerable<AssessmentDto> response =
            await Mediator.Send(new GetAssessmentsByAssignedProfessionalQuery(professionalSub), cancellationToken);

        return Ok(response);
    }
}
