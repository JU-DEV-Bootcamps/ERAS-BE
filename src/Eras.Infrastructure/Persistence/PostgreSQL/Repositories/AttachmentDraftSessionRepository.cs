using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public sealed class AttachmentDraftSessionRepository(AppDbContext Context)
    : BaseRepository<AttachmentDraftSession, AttachmentDraftSessionEntity>(
        Context, AttachmentDraftSessionMapper.ToDomain, AttachmentDraftSessionMapper.ToPersistence), IAttachmentDraftSessionRepository
{
    public async Task DeleteByIdAsync(int Id)
    {
        // FindAsync returns the same already-tracked instance if one exists (e.g. from a prior
        // GetByIdAsync in this request), instead of the fresh instance DeleteAsync(entity) would
        // build via the mapper — avoiding EF's "already tracked" conflict on the same key.
        AttachmentDraftSessionEntity? entity = await _context.Set<AttachmentDraftSessionEntity>().FindAsync(Id);
        if (entity is not null)
        {
            _context.Set<AttachmentDraftSessionEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IReadOnlyCollection<AttachmentDraftSession>> GetOrphanedAsync(
        DateTime OlderThan, CancellationToken CancellationToken = default)
    {
        List<AttachmentDraftSessionEntity> orphaned = await _context.Set<AttachmentDraftSessionEntity>()
            .Where(Session => Session.CreatedAt < OlderThan)
            .Where(Session => !_context.Attachments.Any(Attachment =>
                Attachment.EntityType == AttachmentDraftSession.AttachmentEntityType && Attachment.EntityId == Session.Id))
            .AsNoTracking()
            .ToListAsync(CancellationToken);

        return orphaned.Select(AttachmentDraftSessionMapper.ToDomain).ToList();
    }
}
