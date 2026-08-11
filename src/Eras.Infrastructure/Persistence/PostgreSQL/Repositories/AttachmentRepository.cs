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
}
