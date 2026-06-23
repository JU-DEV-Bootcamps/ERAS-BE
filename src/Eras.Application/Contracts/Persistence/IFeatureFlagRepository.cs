using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IFeatureFlagRepository : IBaseRepository<FeatureFlag>
{
    Task<FeatureFlag?> GetByNameAsync(string Name);
}