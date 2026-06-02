using System.Diagnostics.CodeAnalysis;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Eras.Application.Models;
using Microsoft.Extensions.Options;
using Eras.Application.Contracts.Infrastructure;

namespace Eras.Api.Controllers.AssessmentManagement;

[ApiController]
[Route("api/v1/assessments")]
[Authorize]
[ExcludeFromCodeCoverage]
public class AssessmentsController(IMediator Mediator, ILogger<AssessmentsController> Logger, IOptions<FileStorageSettings> FileStorageOptions, IFileStorageService FileStorage) : ControllerBase 
{
    private readonly FileStorageSettings _fileStorageSettings = FileStorageOptions.Value;
    private readonly IFileStorageService _fileStorage = FileStorage; 

    [HttpGet("{id:int}")]
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
    [ProducesResponseType(typeof(IReadOnlyCollection<AssessmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AssessmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetAllRemissionsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("by-student/{studentId:int}")]
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
    [ProducesResponseType(typeof(AssessmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssessmentDto>> Update(
        int id,
        [FromBody] AssessmentDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.Id == default)
            return BadRequest("Route id must match payload id.");

        var response = await Mediator.Send(new UpdateRemissionCommand(dto with { Id = id }), cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
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
    [ProducesResponseType(typeof(IReadOnlyCollection<InterventionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<InterventionDto>>> GetInterventions(
        int id,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetInterventionsByAssessmentQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpPost("interventions")]
    [ProducesResponseType(typeof(InterventionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InterventionDto>> AddIntervention(
        [FromBody] AddInterventionDto dto,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new AddInterventionCommand(dto.AssessmentId, dto.Intervention), cancellationToken);
        return Created(string.Empty, response);
    }

    [HttpPut("{id:int}/interventions")]
    [ProducesResponseType(typeof(IReadOnlyCollection<InterventionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<InterventionDto>>> UpsertInterventions(
        int id,
        [FromBody] IReadOnlyCollection<InterventionDto> interventions,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new UpsertInterventionsCommand(id, interventions), cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:int}/interventions/{interventionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIntervention(
        int id,
        int interventionId,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteInterventionCommand(id, interventionId), cancellationToken);
        return NoContent();
    }

    [HttpPost("interventions/{interventionId}/attachments")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        var result = await Mediator.Send(
            new UploadInterventionAttachmentsCommand(interventionId, fileStreams),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("interventions/{interventionId}/attachments/{fileName}")]
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
}