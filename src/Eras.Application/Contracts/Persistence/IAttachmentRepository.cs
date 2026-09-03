using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IAttachmentRepository : IBaseRepository<Attachment>
{
    Task<IEnumerable<Attachment>> GetByEntityAsync(string EntityType, int EntityId);
    Task<Attachment?> GetByContentHashAsync(string EntityType, int EntityId, string ContentHash);
    Task<int> CountByEntityAsync(string EntityType, int EntityId);

    /// <summary>
    /// Bulk-reassigns every attachment row owned by (<paramref name="FromEntityType"/>,
    /// <paramref name="FromEntityId"/>) to (<paramref name="ToEntityType"/>,
    /// <paramref name="ToEntityId"/>) and stamps <see cref="Attachment.StorageRelocationPendingAt"/>
    /// on each, as a single UPDATE statement (EF Core's <c>ExecuteUpdateAsync</c>).
    /// </summary>
    /// <returns>The number of attachment rows reassigned.</returns>
    Task<int> ReassignEntityAsync(
        string FromEntityType,
        int FromEntityId,
        string ToEntityType,
        int ToEntityId,
        DateTime RelocationPendingAt);
}
