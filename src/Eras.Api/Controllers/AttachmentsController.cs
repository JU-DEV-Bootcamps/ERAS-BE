using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AttachmentManagement;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

/// <summary>
/// Generic, entity-agnostic attachment endpoints (User Story 1.4) — upload/list/download/delete
/// for any entity type on <c>AttachmentEntityTypeRegistry</c>'s whitelist. Does not replace the
/// existing Intervention-specific endpoints on <c>AssessmentsController</c>; that migration is
/// User Story 1.6.
/// </summary>
[ApiController]
[Route("api/v1/attachments")]
[Authorize]
[ExcludeFromCodeCoverage]
public class AttachmentsController(IAttachmentService AttachmentService, ILogger<AttachmentsController> Logger) : ControllerBase
{
    private readonly IAttachmentService _attachmentService = AttachmentService;
    private readonly ILogger<AttachmentsController> _logger = Logger;

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AttachmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyCollection<AttachmentDto>>> Upload(
        [FromQuery] string entityType,
        [FromQuery] int entityId,
        [FromForm] IFormFileCollection files,
        CancellationToken cancellationToken)
    {
        if (files.Count == 0)
            return BadRequest("No files provided.");

        string createdBy = GetCurrentUserId();

        var openedFiles = files
            .Select(file => (Stream: (Stream)file.OpenReadStream(), file.FileName))
            .ToList();

        try
        {
            IReadOnlyCollection<AttachmentDto> results = await _attachmentService.UploadAttachmentsAsync(
                entityType, entityId, openedFiles, createdBy, cancellationToken);
            return Created(string.Empty, results);
        }
        finally
        {
            foreach ((Stream stream, _) in openedFiles)
                await stream.DisposeAsync();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AttachmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<AttachmentDto>>> List(
        [FromQuery] string entityType,
        [FromQuery] int entityId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<AttachmentDto> attachments =
            await _attachmentService.ListAttachmentsAsync(entityType, entityId, cancellationToken);
        return Ok(attachments);
    }

    [HttpGet("{id:int}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(int id, CancellationToken cancellationToken)
    {
        // Prefer a direct-access URL when the active storage provider offers one (e.g. a future
        // Swift Temporary URL) so the file doesn't have to be streamed through this server.
        string? url = await _attachmentService.GetAttachmentUrlAsync(id, cancellationToken);
        if (url is not null)
            return Redirect(url);

        (Stream stream, string? mimeType, string? originalFileName) =
            await _attachmentService.DownloadAttachmentAsync(id, cancellationToken);

        return File(stream, mimeType ?? "application/octet-stream", originalFileName, enableRangeProcessing: false);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _attachmentService.DeleteAttachmentAsync(id, cancellationToken);
        _logger.LogInformation("Attachment {AttachmentId} deleted.", id);
        return NoContent();
    }

    private string GetCurrentUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? "unknown";
}
