using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

[ExcludeFromCodeCoverage]
public sealed class AttachmentRepository(AppDbContext Context) : BaseRepository<Attachment, Attachment>
    (Context, X => X, X => X), IAttachmentRepository
{
    public async Task<IEnumerable<Attachment>> GetByEntityAsync(string EntityType, int EntityId)
    {
        return await _context.Attachments
            .Where(Attachment => Attachment.EntityType == EntityType && Attachment.EntityId == EntityId)
            .ToListAsync();
    }

    public async Task<Attachment?> GetByContentHashAsync(string EntityType, int EntityId, string ContentHash)
    {
        return await _context.Attachments.FirstOrDefaultAsync(
            Attachment => Attachment.EntityType == EntityType
                && Attachment.EntityId == EntityId
                && Attachment.ContentHash == ContentHash
        );
    }

    public async Task<int> CountByEntityAsync(string EntityType, int EntityId)
    {
        return await _context.Attachments.CountAsync(
            Attachment => Attachment.EntityType == EntityType && Attachment.EntityId == EntityId
        );
    }
}
