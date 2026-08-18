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
public class AttachmentsController(IAttachmentService AttachmentService, ILogger<AttachmentsController> Logger) : ControllerBase
{
    private readonly IAttachmentService _attachmentService = AttachmentService;
    private readonly ILogger<AttachmentsController> _logger = Logger;

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AttachmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyCollection<AttachmentDto>>> UploadAsync(
        [FromQuery] string EntityType,
        [FromQuery] int EntityId,
        [FromForm] IFormFileCollection Files,
        CancellationToken CancellationToken)
    {
        if (Files.Count == 0)
            return BadRequest("No files provided.");

        string createdBy = GetCurrentUserId();

        var openedFiles = Files
            .Select(File => (Stream: (Stream)File.OpenReadStream(), File.FileName))
            .ToList();

        try
        {
            IReadOnlyCollection<AttachmentDto> results = await _attachmentService.UploadAttachmentsAsync(
                EntityType, EntityId, openedFiles, createdBy, CancellationToken);
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
    public async Task<ActionResult<IReadOnlyCollection<AttachmentDto>>> ListAsync(
        [FromQuery] string EntityType,
        [FromQuery] int EntityId,
        CancellationToken CancellationToken)
    {
        IReadOnlyCollection<AttachmentDto> attachments =
            await _attachmentService.ListAttachmentsAsync(EntityType, EntityId, CancellationToken);
        return Ok(attachments);
    }

    [HttpGet("{id:int}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAsync(int Id, CancellationToken CancellationToken)
    {
        // Prefer a direct-access URL when the active storage provider offers one (e.g. a future
        // Swift Temporary URL) so the file doesn't have to be streamed through this server.
        var url = await _attachmentService.GetAttachmentUrlAsync(Id, CancellationToken);
        if (url is not null)
            return Redirect(url);

        (Stream stream, var mimeType, var originalFileName) =
            await _attachmentService.DownloadAttachmentAsync(Id, CancellationToken);

        return File(stream, mimeType ?? "application/octet-stream", originalFileName, enableRangeProcessing: false);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int Id, CancellationToken CancellationToken)
    {
        await _attachmentService.DeleteAttachmentAsync(Id, CancellationToken);
        _logger.LogInformation("Attachment {AttachmentId} deleted.", Id);
        return NoContent();
    }

    private string GetCurrentUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? "unknown";
}
