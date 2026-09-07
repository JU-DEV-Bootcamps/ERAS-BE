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

    /// <summary>
    /// Returns every attachment of <paramref name="EntityType"/> created before
    /// <paramref name="OlderThan"/> — used by the temp-attachment cleanup sweep to find expired,
    /// never-claimed draft uploads.
    /// </summary>
    Task<IReadOnlyCollection<Attachment>> GetStaleByEntityTypeAsync(
        string EntityType, DateTime OlderThan, CancellationToken CancellationToken = default);

    /// <summary>
    /// Deletes the attachment row by id. Unlike the inherited <c>DeleteAsync(entity)</c>, this is
    /// safe to call after a prior <c>GetByIdAsync</c> for the same id in the same
    /// <c>AppDbContext</c> — <c>DeleteAsync</c> re-maps the domain object into a fresh persistence
    /// instance and conflicts with the one already tracked from that earlier read.
    /// </summary>
    Task DeleteByIdAsync(int Id);
}
