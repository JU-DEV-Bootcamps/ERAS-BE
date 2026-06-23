using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

[ExcludeFromCodeCoverage]
public sealed class FeatureFlagRepository(AppDbContext Context) : BaseRepository<FeatureFlag, FeatureFlag>
    (Context, X => X, X => X), IFeatureFlagRepository
{
    public async Task<FeatureFlag?> GetByNameAsync(string Name)
    {
        FeatureFlag? featureFlag = await _context.FeatureFlags.FirstOrDefaultAsync(
            FeatureFlag => FeatureFlag.Name == Name
        );
        return featureFlag;
    }
}
