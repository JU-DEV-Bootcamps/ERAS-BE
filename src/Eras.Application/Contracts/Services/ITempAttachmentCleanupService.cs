namespace Eras.Application.Contracts.Services;

/// <summary>
/// Sweeps expired "Temp" (draft-session-staged, never-claimed) attachments — e.g. left behind
/// when a user closes a create/edit form without saving. Deletes each one's metadata and physical
/// file via <see cref="IAttachmentService.DeleteAttachmentAsync"/>, then deletes any draft session
/// row that's past its TTL and has nothing left staged under it. Driven on a schedule by
/// <c>Eras.Infrastructure.BackgroundProcessing.TempAttachmentCleanupJob</c>.
/// </summary>
public interface ITempAttachmentCleanupService
{
    Task RunAsync(CancellationToken CancellationToken = default);
}
