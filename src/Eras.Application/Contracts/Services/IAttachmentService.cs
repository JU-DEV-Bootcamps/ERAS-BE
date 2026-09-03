using Eras.Application.DTOs.AttachmentManagement;

namespace Eras.Application.Contracts.Services;

/// <summary>
/// Generic, entity-agnostic attachment upload/list/download/delete service backing the
/// `attachments` table and <c>IFileStorageService</c>. Any entity type on the
/// <c>AttachmentEntityTypeRegistry</c> whitelist can use this. Attributes every upload it creates
/// to the caller reported by <c>IUserIdentityProvider</c> — never passed in by callers.
/// </summary>
public interface IAttachmentService
{
    /// <summary>
    /// Uploads a file for the given entity. Deduplicates by SHA-256 content hash within the same
    /// entity (an existing match is returned as-is, nothing re-saved), and rejects the upload once
    /// the entity is at its configured max-attachment count. Compensates for a metadata-write
    /// failure after a successful physical save by deleting the orphaned file.
    /// </summary>
    /// <exception cref="Eras.Error.Bussiness.BussinessException">
    /// `entityType` is not on the whitelist (400); the file extension isn't allowed (400); the
    /// file exceeds `FileStorageSettings.MaxFileSizeBytes` (400); the file's actual content
    /// (magic bytes) doesn't match what its extension claims (400) — extension alone is never the
    /// sole validation signal; or the entity is already at its max-attachment count (409).
    /// </exception>
    Task<AttachmentDto> UploadAttachmentAsync(
        string EntityType,
        int EntityId,
        Stream FileStream,
        string FileName,
        CancellationToken CancellationToken = default);

    /// <summary>
    /// Uploads multiple files for the same entity as one batch. Unlike calling
    /// <see cref="UploadAttachmentAsync"/> once per file, a failure partway through rolls back
    /// every attachment *this call* newly created before re-throwing — attachments that were
    /// dedup-matched (already existed) are left untouched since this call didn't create them.
    /// Files are still processed sequentially, so earlier files' successful uploads count toward
    /// the max-attachment check for later files in the same batch.
    /// </summary>
    /// <exception cref="Eras.Error.Bussiness.BussinessException">
    /// `entityType` is not on the whitelist (400); any file's extension isn't allowed (400), checked
    /// up front for the whole batch before any file is saved; a per-file failure partway through
    /// (oversized file, content/extension mismatch, or the entity reaching its max-attachment count)
    /// triggers the rollback described above before re-throwing.
    /// </exception>
    Task<IReadOnlyCollection<AttachmentDto>> UploadAttachmentsAsync(
        string EntityType,
        int EntityId,
        IReadOnlyCollection<(Stream FileStream, string FileName)> Files,
        CancellationToken CancellationToken = default);

    /// <exception cref="Eras.Error.Bussiness.BussinessException">`entityType` is not on the whitelist (400).</exception>
    Task<IReadOnlyCollection<AttachmentDto>> ListAttachmentsAsync(
        string EntityType,
        int EntityId,
        CancellationToken CancellationToken = default);

    /// <summary>Streams the attachment's content directly. Prefer <see cref="GetAttachmentUrlAsync"/> when a direct URL is available.</summary>
    /// <exception cref="Eras.Error.Bussiness.NotFoundException">No attachment exists for <paramref name="attachmentId"/>.</exception>
    Task<(Stream Stream, string? MimeType, string? OriginalFileName)> DownloadAttachmentAsync(
        int AttachmentId,
        CancellationToken CancellationToken = default);

    /// <summary>Direct-access URL for the attachment's content, or null if the active storage provider has no such concept (see <c>IFileStorageService.GetUrlAsync</c>).</summary>
    /// <exception cref="Eras.Error.Bussiness.NotFoundException">No attachment exists for <paramref name="attachmentId"/>.</exception>
    Task<string?> GetAttachmentUrlAsync(int AttachmentId, CancellationToken CancellationToken = default);

    /// <exception cref="Eras.Error.Bussiness.NotFoundException">No attachment exists for <paramref name="attachmentId"/>.</exception>
    Task DeleteAttachmentAsync(int AttachmentId, CancellationToken CancellationToken = default);

    /// <summary>
    /// Claims every attachment staged under a draft session for its real owning entity
    /// </summary>
    /// <param name="RequestedBy">
    /// The caller's identity, checked against the draft session's <c>CreatedBy</c>
    /// </param>
    /// <exception cref="Eras.Error.Bussiness.NotFoundException">
    /// No draft session exists for <paramref name="DraftSessionId"/>, it wasn't created by
    /// <paramref name="RequestedBy"/>, or it has no staged attachments left to claim — the caller
    /// is told its staged files are gone rather than the call silently doing nothing.
    /// </exception>
    /// <exception cref="Eras.Error.Bussiness.BussinessException">
    /// `ToEntityType` is not on the whitelist (400); claiming the drafted attachments would push
    /// the target past its configured max-attachment count (409), counting its existing
    /// attachments plus the ones being claimed.
    /// </exception>
    Task ClaimDraftAttachmentsAsync(
        int DraftSessionId,
        string ToEntityType,
        int ToEntityId,
        string RequestedBy,
        CancellationToken CancellationToken = default);
}
