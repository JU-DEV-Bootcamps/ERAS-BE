using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;

public sealed class AssessmentActivityRepository(AppDbContext Context) : IAssessmentActivityRepository
{
    public async Task AddRangeAsync(
        IReadOnlyCollection<AssessmentActivity> Activities,
        CancellationToken CancellationToken = default)
    {
        if (Activities.Count == 0)
            return;

        Context.Set<AssessmentActivity>().AddRange(Activities);
        await Context.SaveChangesAsync(CancellationToken);
    }

    public async Task<IReadOnlyList<AssessmentActivity>> GetByAssessmentIdAsync(
        int AssessmentId,
        CancellationToken CancellationToken = default)
    {
        return await Context.Set<AssessmentActivity>()
            .AsNoTracking()
            .Where(Activity => Activity.AssessmentId == AssessmentId)
            .ToListAsync(CancellationToken);
    }
}
