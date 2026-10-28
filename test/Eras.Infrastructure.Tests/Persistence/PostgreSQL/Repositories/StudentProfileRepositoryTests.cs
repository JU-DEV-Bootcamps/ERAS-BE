using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentProfileRepositoryTests : RepositoryTestBase
{
    private static StudentProfile NewProfile(int StudentId, string IdPassport) => new()
    {
        StudentId = StudentId,
        FirstName = "Ana",
        LastName = "Pérez",
        IdPassportNumber = IdPassport,
        Audit = new AuditInfo { CreatedBy = "tester", CreatedAt = DateTime.UtcNow },
    };

    private static async Task Seed(AppDbContext Context, params StudentProfile[] Profiles)
    {
        Context.StudentProfiles.AddRange(Profiles);
        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByStudentIdAsync_ExistingProfile_ReturnsIt()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, NewProfile(1, "A1"), NewProfile(2, "B2"));
        StudentProfileRepository repository = new(context);

        StudentProfile? result = await repository.GetByStudentIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("B2", result!.IdPassportNumber);
    }

    [Fact]
    public async Task GetByStudentIdAsync_UnknownStudent_ReturnsNull()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, NewProfile(1, "A1"));
        StudentProfileRepository repository = new(context);

        Assert.Null(await repository.GetByStudentIdAsync(99));
    }

    [Fact]
    public async Task ExistsByIdPassportNumberAsync_MatchingNumber_ReturnsTrue()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, NewProfile(1, "A1"));
        StudentProfileRepository repository = new(context);

        Assert.True(await repository.ExistsByIdPassportNumberAsync("A1"));
        Assert.False(await repository.ExistsByIdPassportNumberAsync("ZZ"));
    }

    [Fact]
    public async Task ExistsByIdPassportNumberAsync_ExcludingOwner_IgnoresOwnProfile()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, NewProfile(1, "A1"), NewProfile(2, "B2"));
        StudentProfileRepository repository = new(context);

        Assert.False(await repository.ExistsByIdPassportNumberAsync("A1", 1));
        Assert.True(await repository.ExistsByIdPassportNumberAsync("A1", 2));
    }

    [Fact]
    public async Task SaveChanges_DetachedProfileWithCurrentCourses_PersistsThem()
    {
        // Regression: courses were an owned JSON collection, and saving a detached profile
        // (how the repositories update) failed on the owned type's shadow ordinal key.
        using AppDbContext context = CreateContext();
        StudentProfile profile = NewProfile(1, "A1");
        profile.CurrentCourses = [new StudentCurrentCourse { Name = "Algorithms", Group = "G1", Section = "A" }];
        profile.IdentifiedRisks = ["Low attendance"];
        await Seed(context, profile);
        context.ChangeTracker.Clear();

        StudentProfile detached = NewProfile(1, "A1");
        detached.Id = profile.Id;
        detached.CurrentCourses =
        [
            new StudentCurrentCourse { Name = "Algorithms", Group = "G2", Section = "B" },
            new StudentCurrentCourse { Name = "Databases" },
        ];
        context.StudentProfiles.Update(detached);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        StudentProfile? reloaded = await new StudentProfileRepository(context).GetByStudentIdAsync(1);

        Assert.Equal(2, reloaded!.CurrentCourses!.Count);
        Assert.Equal("G2", reloaded.CurrentCourses[0].Group);
        Assert.Null(reloaded.CurrentCourses[1].Group);
    }

    [Fact]
    public async Task GetStudentIdsWithProfileAsync_ReturnsOnlyStudentsThatHaveOne()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, NewProfile(1, "A1"), NewProfile(3, "C3"));
        StudentProfileRepository repository = new(context);

        HashSet<int> result = await repository.GetStudentIdsWithProfileAsync([1, 2, 3, 4]);

        Assert.Equal(new HashSet<int> { 1, 3 }, result);
    }
}
