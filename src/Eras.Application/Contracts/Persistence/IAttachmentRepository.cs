using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IAttachmentRepository : IBaseRepository<Attachment>
{
    Task<IEnumerable<Attachment>> GetByEntityAsync(string EntityType, int EntityId);
    Task<Attachment?> GetByContentHashAsync(string EntityType, int EntityId, string ContentHash);
    Task<int> CountByEntityAsync(string EntityType, int EntityId);
}
