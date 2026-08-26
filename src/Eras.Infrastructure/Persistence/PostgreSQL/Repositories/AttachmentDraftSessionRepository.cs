using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public sealed class AttachmentDraftSessionRepository(AppDbContext Context)
    : BaseRepository<AttachmentDraftSession, AttachmentDraftSessionEntity>(
        Context, AttachmentDraftSessionMapper.ToDomain, AttachmentDraftSessionMapper.ToPersistence), IAttachmentDraftSessionRepository
{
}
