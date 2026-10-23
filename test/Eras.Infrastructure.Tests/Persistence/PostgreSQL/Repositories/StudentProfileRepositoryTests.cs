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
    public async Task GetStudentIdsWithProfileAsync_ReturnsOnlyStudentsThatHaveOne()
    {
        using AppDbContext context = CreateContext();
        await Seed(context, NewProfile(1, "A1"), NewProfile(3, "C3"));
        StudentProfileRepository repository = new(context);

        HashSet<int> result = await repository.GetStudentIdsWithProfileAsync([1, 2, 3, 4]);

        Assert.Equal(new HashSet<int> { 1, 3 }, result);
    }
}
