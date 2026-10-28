using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentRepositorySearchTests : RepositoryTestBase
{
    private static StudentRepository CreateRepository(AppDbContext context) =>
        new(context, new Mock<IAnswerRiskValidator>().Object);

    private static async Task SeedStudentsAsync(AppDbContext context, params string[] names)
    {
        for (int index = 0; index < names.Length; index++)
        {
            context.Students.Add(new StudentEntity
            {
                Uuid = Guid.NewGuid().ToString(),
                Name = names[index],
                Email = $"student{index}@test.com",
                Audit = new AuditInfo { CreatedBy = "tester", CreatedAt = DateTime.UtcNow },
            });
        }

        await context.SaveChangesAsync();
    }

    [Fact]
    [Trait("Change", "642-search-students-by-name")]
    [Trait("AC", "642-search-students-by-name/AC-1")]
    public async Task GetPagedAsyncWithJoins_SearchTextInName_ReturnsMatchesIgnoringCaseAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedStudentsAsync(context, "Ana Torres", "Luis Anaya", "Pedro Gil");
        StudentRepository repository = CreateRepository(context);

        var result = await repository.GetPagedAsyncWithJoins(1, 10, "ANA");

        Assert.Equal(["Ana Torres", "Luis Anaya"], result.Students.Select(student => student.Name));
        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    [Trait("Change", "642-search-students-by-name")]
    [Trait("AC", "642-search-students-by-name/AC-2")]
    public async Task GetPagedAsyncWithJoins_SearchWithoutAccents_MatchesAccentedNamesAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedStudentsAsync(context, "José Pérez", "Maria Ñúñez");
        StudentRepository repository = CreateRepository(context);

        var byPlainText = await repository.GetPagedAsyncWithJoins(1, 10, "jose");
        var byAccentedText = await repository.GetPagedAsyncWithJoins(1, 10, "María");

        Assert.Equal(["José Pérez"], byPlainText.Students.Select(student => student.Name));
        Assert.Equal(["Maria Ñúñez"], byAccentedText.Students.Select(student => student.Name));
    }

    [Fact]
    [Trait("Change", "642-search-students-by-name")]
    [Trait("AC", "642-search-students-by-name/AC-3")]
    public async Task GetPagedAsyncWithJoins_SearchWithWildcardCharacters_MatchesLiterallyAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedStudentsAsync(context, "Ana Torres", "Ana_100% Gil");
        StudentRepository repository = CreateRepository(context);

        var result = await repository.GetPagedAsyncWithJoins(1, 10, "_100%");

        Assert.Equal(["Ana_100% Gil"], result.Students.Select(student => student.Name));
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    [Trait("Change", "642-search-students-by-name")]
    [Trait("AC", "642-search-students-by-name/AC-4")]
    public async Task GetPagedAsyncWithJoins_BlankSearch_ReturnsFullListAsync()
    {
        using AppDbContext context = CreateContext();
        await SeedStudentsAsync(context, Enumerable.Range(1, 12).Select(index => $"Student {index:D2}").ToArray());
        StudentRepository repository = CreateRepository(context);

        var blank = await repository.GetPagedAsyncWithJoins(1, 50, "   ");
        var none = await repository.GetPagedAsyncWithJoins(1, 50, null);

        Assert.Equal(12, blank.TotalCount);
        Assert.Equal(12, blank.Students.Count());
        Assert.Equal(none.TotalCount, blank.TotalCount);
    }

    [Fact]
    [Trait("Change", "642-search-students-by-name")]
    [Trait("AC", "642-search-students-by-name/AC-5")]
    public async Task GetPagedAsyncWithJoins_SearchOverManyPages_PagesMatchesAndCountsAllAsync()
    {
        using AppDbContext context = CreateContext();
        List<string> names = [];
        names.AddRange(Enumerable.Range(1, 95).Select(index => $"Alumno {index:D3}"));
        names.AddRange(Enumerable.Range(1, 25).Select(index => $"Zoe García {index:D2}"));
        await SeedStudentsAsync(context, [.. names]);
        StudentRepository repository = CreateRepository(context);

        var firstPage = await repository.GetPagedAsyncWithJoins(1, 10, "garcia");
        var thirdPage = await repository.GetPagedAsyncWithJoins(3, 10, "garcia");

        Assert.Equal(25, firstPage.TotalCount);
        Assert.Equal(10, firstPage.Students.Count());
        Assert.Equal(
            firstPage.Students.Select(student => student.Name).OrderBy(name => name, StringComparer.Ordinal),
            firstPage.Students.Select(student => student.Name));
        Assert.Equal(5, thirdPage.Students.Count());
        Assert.Equal("Zoe García 25", thirdPage.Students.Last().Name);
    }
}
