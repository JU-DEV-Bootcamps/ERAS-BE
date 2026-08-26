using Eras.Application.DTOs.AttachmentManagement;

namespace Eras.Application.Contracts.Services;

/// <summary>
/// Hands out draft session ids (see <c>Eras.Domain.Entities.AttachmentDraftSession</c>) so a
/// caller can stage attachments — via the generic `entityType=Temp` attachment endpoints — before
/// their real owning entity exists. 
/// </summary>
public interface IAttachmentDraftSessionService
{
    /// <summary>Creates a new draft session scoped to <paramref name="CreatedBy"/>.</summary>
    Task<DraftSessionDto> CreateDraftSessionAsync(string CreatedBy, CancellationToken CancellationToken = default);
}
