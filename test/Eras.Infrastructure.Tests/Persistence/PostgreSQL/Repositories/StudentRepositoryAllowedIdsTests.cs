using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentRepositoryAllowedIdsTests : RepositoryTestBase
{
    private static StudentRepository CreateRepository(AppDbContext Context) =>
        new(Context, new Mock<IAnswerRiskValidator>().Object);

    private static async Task SeedAsync(AppDbContext Context, params string[] Names)
    {
        for (int index = 0; index < Names.Length; index++)
        {
            Context.Students.Add(new StudentEntity
            {
                Id = index + 1,
                Uuid = Guid.NewGuid().ToString(),
                Name = Names[index],
                Email = $"student{index}@test.com",
                Audit = new AuditInfo { CreatedBy = "tester", CreatedAt = DateTime.UtcNow },
            });
        }

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetPagedAsyncWithJoins_AllowedIds_ReturnsOnlyThoseStudentsAndCountsThemAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedAsync(context, "Ana Torres", "Luis Anaya", "Pedro Gil");

        var result = await CreateRepository(context).GetPagedAsyncWithJoins(1, 10, null, [1, 3]);

        Assert.Equal(["Ana Torres", "Pedro Gil"], result.Students.Select(Student => Student.Name));
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsyncWithJoins_AllowedIdsAndSearch_AppliesBothAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedAsync(context, "Ana Torres", "Luis Anaya", "Pedro Gil");

        var result = await CreateRepository(context).GetPagedAsyncWithJoins(1, 10, "ana", [2, 3]);

        Assert.Equal(["Luis Anaya"], result.Students.Select(Student => Student.Name));
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsyncWithJoins_EmptyAllowedIds_ReturnsNoStudentsAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedAsync(context, "Ana Torres");

        var result = await CreateRepository(context).GetPagedAsyncWithJoins(1, 10, null, []);

        Assert.Empty(result.Students);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsyncWithJoins_NullAllowedIds_ReturnsEveryStudentAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedAsync(context, "Ana Torres", "Luis Anaya");

        var result = await CreateRepository(context).GetPagedAsyncWithJoins(1, 10, null, null);

        Assert.Equal(2, result.TotalCount);
    }
}
