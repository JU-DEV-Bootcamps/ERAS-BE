using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Domain.Entities;

namespace Eras.Application.Services;

public sealed class AttachmentDraftSessionService(IAttachmentDraftSessionRepository Repository) : IAttachmentDraftSessionService
{
    private readonly IAttachmentDraftSessionRepository _repository = Repository;

    public async Task<DraftSessionDto> CreateDraftSessionAsync(string CreatedBy, CancellationToken CancellationToken = default)
    {
        var session = new AttachmentDraftSession { CreatedBy = CreatedBy };
        AttachmentDraftSession persisted = await _repository.AddAsync(session);
        return new DraftSessionDto { DraftId = persisted.Id };
    }
}
