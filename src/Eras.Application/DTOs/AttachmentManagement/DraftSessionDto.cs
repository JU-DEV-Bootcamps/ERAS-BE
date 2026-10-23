namespace Eras.Application.DTOs.AttachmentManagement;

/// <summary>Response shape for <c>POST /api/v1/attachments/drafts</c>.</summary>
public sealed record DraftSessionDto
{
    /// <summary>
    /// The new draft session's id. Use it as `entityId` (with `entityType=Temp`) against the
    /// generic attachment endpoints to stage files before the real owning entity exists.
    /// </summary>
    public required int DraftId { get; init; }
}
