using Eras.Domain.Entities.FeatureFlagManagement;

namespace Eras.Application.Contracts.Persistence;

public interface IFeatureFlagRepository : IBaseRepository<FeatureFlag>
{
    Task<FeatureFlag?> GetByNameAsync(string Name);
    Task<FeatureFlag?> GetByIdNoTrackingAsync(int Id);
}