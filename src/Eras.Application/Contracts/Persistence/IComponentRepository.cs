
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IComponentRepository : IBaseRepository<Component>
    {
        Task<Component?> GetByNameAsync(string name);
    }
}