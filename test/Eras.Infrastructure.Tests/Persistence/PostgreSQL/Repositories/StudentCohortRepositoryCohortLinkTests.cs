using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentCohortRepositoryCohortLinkTests : RepositoryTestBase
{
    private static async Task Seed(AppDbContext Context, params (int StudentId, int CohortId)[] Links)
    {
        Context.StudentCohorts.AddRange(Links.Select(L => new StudentCohortJoin { StudentId = L.StudentId, CohortId = L.CohortId }));
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetCohortIdByStudentIdAsync_ReturnsTheMostRecentLink()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, (1, 10), (1, 20), (2, 30));

        Assert.Equal(20, await new StudentCohortRepository(context).GetCohortIdByStudentIdAsync(1));
    }

    [Fact]
    public async Task GetCohortIdByStudentIdAsync_StudentWithoutCohort_ReturnsNull()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, (2, 30));

        Assert.Null(await new StudentCohortRepository(context).GetCohortIdByStudentIdAsync(1));
    }

    [Fact]
    public async Task ChangeCohortAsync_MovesTheMostRecentLink()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, (1, 10), (1, 20));

        await new StudentCohortRepository(context).ChangeCohortAsync(1, 99);

        List<int> cohorts = await context.StudentCohorts.Where(L => L.StudentId == 1)
            .OrderBy(L => L.Id).Select(L => L.CohortId).ToListAsync();
        Assert.Equal([10, 99], cohorts);
    }

    [Fact]
    public async Task ChangeCohortAsync_StudentWithoutLink_CreatesIt()
    {
        using AppDbContext context = CreateContext();

        await new StudentCohortRepository(context).ChangeCohortAsync(1, 5);

        Assert.Equal(5, await new StudentCohortRepository(context).GetCohortIdByStudentIdAsync(1));
    }

    [Fact]
    public async Task ChangeCohortAsync_AlreadyInThatCohort_ChangesNothing()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, (1, 10), (1, 20));

        await new StudentCohortRepository(context).ChangeCohortAsync(1, 10);

        List<int> cohorts = await context.StudentCohorts.OrderBy(L => L.Id).Select(L => L.CohortId).ToListAsync();
        Assert.Equal([10, 20], cohorts);
    }
}
