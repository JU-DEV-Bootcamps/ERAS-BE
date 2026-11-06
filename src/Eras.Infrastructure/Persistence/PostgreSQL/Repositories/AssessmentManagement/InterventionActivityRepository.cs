using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

public sealed class InterventionActivityRepository(AppDbContext Context) : IInterventionActivityRepository
{
    public async Task AddRangeAsync(
        IReadOnlyCollection<InterventionActivity> Activities,
        CancellationToken CancellationToken = default)
    {
        if (Activities.Count == 0)
            return;

        Context.Set<InterventionActivity>().AddRange(Activities);
        await Context.SaveChangesAsync(CancellationToken);
    }

    public async Task<IReadOnlyList<InterventionActivity>> GetByInterventionIdAsync(
        int InterventionId,
        CancellationToken CancellationToken = default)
    {
        return await Context.Set<InterventionActivity>()
            .AsNoTracking()
            .Where(Activity => Activity.InterventionId == InterventionId)
            .ToListAsync(CancellationToken);
    }
}
