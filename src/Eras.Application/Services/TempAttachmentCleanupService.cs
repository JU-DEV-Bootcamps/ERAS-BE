using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.Models;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eras.Application.Services;

public sealed class TempAttachmentCleanupService(
    IAttachmentRepository AttachmentRepository,
    IAttachmentDraftSessionRepository DraftSessionRepository,
    IAttachmentService AttachmentService,
    IOptions<FileStorageSettings> Settings,
    ILogger<TempAttachmentCleanupService> Logger) : ITempAttachmentCleanupService
{
    private readonly IAttachmentRepository _attachmentRepository = AttachmentRepository;
    private readonly IAttachmentDraftSessionRepository _draftSessionRepository = DraftSessionRepository;
    private readonly IAttachmentService _attachmentService = AttachmentService;
    private readonly FileStorageSettings _settings = Settings.Value;
    private readonly ILogger<TempAttachmentCleanupService> _logger = Logger;

    public async Task RunAsync(CancellationToken CancellationToken = default)
    {
        DateTime cutoff = DateTime.UtcNow.AddHours(-_settings.TempAttachmentTtlHours);

        int deletedAttachments = await DeleteExpiredAttachmentsAsync(cutoff, CancellationToken);
        int deletedSessions = await DeleteOrphanedSessionsAsync(cutoff, CancellationToken);

        if (deletedAttachments > 0 || deletedSessions > 0)
        {
            _logger.LogInformation(
                "Temp attachment cleanup swept {AttachmentCount} expired attachment(s) and {SessionCount} orphaned draft session(s).",
                deletedAttachments, deletedSessions);
        }
    }

    private async Task<int> DeleteExpiredAttachmentsAsync(DateTime Cutoff, CancellationToken CancellationToken)
    {
        IReadOnlyCollection<Attachment> stale = await _attachmentRepository.GetStaleByEntityTypeAsync(
            AttachmentDraftSession.AttachmentEntityType, Cutoff, CancellationToken);

        int deleted = 0;
        foreach (Attachment attachment in stale)
        {
            try
            {
                // Reuses the service's own delete order (metadata first, then physical file) and
                // orphan-safe compensation — nothing about "this is an expired draft" changes that.
                await _attachmentService.DeleteAttachmentAsync(attachment.Id, CancellationToken);
                deleted++;
            }
            catch (Exception ex)
            {
                // One bad row shouldn't stop the sweep from cleaning up the rest.
                _logger.LogError(ex,
                    "Failed to delete expired temp attachment {AttachmentId} (draft session {DraftSessionId}).",
                    attachment.Id, attachment.EntityId);
            }
        }

        return deleted;
    }

    private async Task<int> DeleteOrphanedSessionsAsync(DateTime Cutoff, CancellationToken CancellationToken)
    {
        // Runs after DeleteExpiredAttachmentsAsync, so a session whose only attachments were just
        // deleted above is already orphaned by the time this queries.
        IReadOnlyCollection<AttachmentDraftSession> orphaned =
            await _draftSessionRepository.GetOrphanedAsync(Cutoff, CancellationToken);

        int deleted = 0;
        foreach (AttachmentDraftSession session in orphaned)
        {
            try
            {
                await _draftSessionRepository.DeleteByIdAsync(session.Id);
                deleted++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete orphaned draft session {DraftSessionId}.", session.Id);
            }
        }

        return deleted;
    }
}
