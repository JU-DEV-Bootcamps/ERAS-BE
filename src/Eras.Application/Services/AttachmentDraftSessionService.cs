using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Domain.Entities;

namespace Eras.Application.Services;

public sealed class AttachmentDraftSessionService(
    IAttachmentDraftSessionRepository Repository,
    IUserIdentityProvider UserIdentityProvider) : IAttachmentDraftSessionService
{
    private readonly IAttachmentDraftSessionRepository _repository = Repository;
    private readonly IUserIdentityProvider _userIdentityProvider = UserIdentityProvider;

    public async Task<DraftSessionDto> CreateDraftSessionAsync(CancellationToken CancellationToken = default)
    {
        var session = new AttachmentDraftSession { CreatedBy = _userIdentityProvider.UserId };
        AttachmentDraftSession persisted = await _repository.AddAsync(session);
        return new DraftSessionDto { DraftId = persisted.Id };
    }
}
