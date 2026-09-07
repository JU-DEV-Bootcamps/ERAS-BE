using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public sealed class AttachmentRepository(AppDbContext Context) : BaseRepository<Attachment, AttachmentEntity>
    (Context, AttachmentMapper.ToDomain, AttachmentMapper.ToPersistence), IAttachmentRepository
{
    public async Task<IEnumerable<Attachment>> GetByEntityAsync(string EntityType, int EntityId)
    {
        List<AttachmentEntity> entities = await _context.Attachments
            .Where(Attachment => Attachment.EntityType == EntityType && Attachment.EntityId == EntityId)
            .ToListAsync();

        return entities.Select(Attachment => Attachment.ToDomain()).ToList();
    }

    public async Task<Attachment?> GetByContentHashAsync(string EntityType, int EntityId, string ContentHash)
    {
        AttachmentEntity? entity = await _context.Attachments.FirstOrDefaultAsync(
            Attachment => Attachment.EntityType == EntityType
                && Attachment.EntityId == EntityId
                && Attachment.ContentHash == ContentHash
        );

        return entity?.ToDomain();
    }

    public async Task<int> CountByEntityAsync(string EntityType, int EntityId)
    {
        return await _context.Attachments.CountAsync(
            Attachment => Attachment.EntityType == EntityType && Attachment.EntityId == EntityId
        );
    }

    public async Task<IReadOnlyCollection<Attachment>> GetStaleByEntityTypeAsync(
        string EntityType, DateTime OlderThan, CancellationToken CancellationToken = default)
    {
        List<AttachmentEntity> entities = await _context.Attachments
            .Where(Attachment => Attachment.EntityType == EntityType && Attachment.CreatedAt < OlderThan)
            .ToListAsync(CancellationToken);

        return entities.Select(Attachment => Attachment.ToDomain()).ToList();
    }

    public async Task DeleteByIdAsync(int Id)
    {
        // FindAsync returns the same already-tracked instance if one exists (e.g. from a prior
        // GetByIdAsync in this request), instead of the fresh instance DeleteAsync(entity) would
        // build via the mapper — avoiding EF's "already tracked" conflict on the same key.
        AttachmentEntity? entity = await _context.Set<AttachmentEntity>().FindAsync(Id);
        if (entity is not null)
        {
            _context.Set<AttachmentEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> ReassignEntityAsync(
        string FromEntityType,
        int FromEntityId,
        string ToEntityType,
        int ToEntityId,
        DateTime RelocationPendingAt)
    {
        return await _context.Attachments
            .Where(Attachment => Attachment.EntityType == FromEntityType && Attachment.EntityId == FromEntityId)
            .ExecuteUpdateAsync(Setters => Setters
                .SetProperty(Attachment => Attachment.EntityType, ToEntityType)
                .SetProperty(Attachment => Attachment.EntityId, ToEntityId)
                .SetProperty(Attachment => Attachment.StorageRelocationPendingAt, RelocationPendingAt));
    }
}
