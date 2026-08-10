using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class CohortRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
    
    private static Mock<DbSet<ErasCalculationsByPollEntity>> CreateMockErasCalculations(IEnumerable<ErasCalculationsByPollEntity> Data)
    {
        var queryable = new TestAsyncEnumerable<ErasCalculationsByPollEntity>(Data);
        var dbSet = new Mock<DbSet<ErasCalculationsByPollEntity>>();

        dbSet
            .As<IAsyncEnumerable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.GetAsyncEnumerator(
                It.IsAny<CancellationToken>()))
            .Returns(() => queryable.GetAsyncEnumerator());

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.Provider)
            .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).Provider);

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.Expression)
            .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).Expression);

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.ElementType)
            .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).ElementType);

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.GetEnumerator())
            .Returns(() => queryable.AsEnumerable().GetEnumerator());
        return dbSet;
    }

    private static CohortRepository CreateRepository(AppDbContext Context)
    {
        return new CohortRepository(Context);
    }

    private static List<ErasCalculationsByPollEntity> CreateErasCalculations()
    {
        return [
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 1,
                CohortName = "Cohort A",
                StudentId = 1,
                StudentName = "Alice",
                PollInstanceId = 101,
                ComponentName = "Engagement",
                AnswerRisk = 4,
                Question = "question2",
                AnswerText = "Response",
                StudentEmail = "ser@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 1,
                CohortName = "Cohort A",
                StudentId = 2,
                StudentName = "Alice",
                PollInstanceId = 101,
                ComponentName = "Attendance",
                AnswerRisk = 3,
                Question = "question3",
                AnswerText = "Response",
                StudentEmail = "ser@mails.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 1,
                CohortName = "Cohort A",
                StudentId = 2,
                StudentName = "Bob",
                PollInstanceId = 102,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                Question = "question5",
                AnswerText = "Response",
                StudentEmail = "ser@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 1,
                CohortName = "Cohort A",
                StudentId = 2,
                StudentName = "Bob",
                PollInstanceId = 102,
                ComponentName = "Attendance",
                AnswerRisk = 1,
                Question = "question25",
                AnswerText = "Response",
                StudentEmail = "ser@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 2,
                CohortName = "Cohort B",
                StudentId = 3,
                StudentName = "Charlie",
                PollInstanceId = 103,
                ComponentName = "Engagement",
                AnswerRisk = 5,
                Question = "question2",
                AnswerText = "Response",
                StudentEmail = "ser@mail.com"
            },

            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 1,
                CohortId = 1,
                CohortName = "Cohort A",
                StudentId = 1,
                StudentName = "Alice",
                PollInstanceId = 91,
                ComponentName = "Engagement",
                AnswerRisk = 1,
                Question = "question2",
                AnswerText = "Response",
                StudentEmail = "ser@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 1,
                CohortId = 1,
                CohortName = "Cohort A",
                StudentId = 4,
                StudentName = "David",
                PollInstanceId = 92,
                ComponentName = "Engagement",
                AnswerRisk = 4,
                Question = "question2",
                AnswerText = "Response",
                StudentEmail = "ser@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-2",
                PollId = 2,
                PollVersion = 3,
                CohortId = 3,
                CohortName = "Cohort C",
                StudentId = 5,
                StudentName = "Emma",
                PollInstanceId = 201,
                ComponentName = "Engagement",
                AnswerRisk = 5,
                Question = "question2",
                AnswerText = "Response",
                StudentEmail = "ser@mail.com"
            }
        ];
    }

    private static async Task SeedAsync(AppDbContext Context)
    {
        Context.Cohorts.AddRange(
            new CohortEntity
            {
                Id = 1,
                Name = "Cohort A",
                CourseCode = "COURSE-A"
            },
            new CohortEntity
            {
                Id = 2,
                Name = "Cohort B",
                CourseCode = "COURSE-B"
            },
            new CohortEntity
            {
                Id = 3,
                Name = "Cohort C",
                CourseCode = "COURSE-C"
            });

        Context.Polls.AddRange(
            new PollEntity
            {
                Id = 1,
                Uuid = "poll-1",
                LastVersion = 2
            },
            new PollEntity
            {
                Id = 2,
                Uuid = "poll-2",
                LastVersion = 3
            });

        CreateErasCalculations();

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByNameAsync_WhenCohortExists_ReturnsCohortAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations(); 
        var calculationsDbSet = CreateMockErasCalculations(calculations); 
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);
        var result = await repository.GetByNameAsync("Cohort A");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cohort A", result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenCohortDoesNotExist_ReturnsNullAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations(); 
        var calculationsDbSet = CreateMockErasCalculations(calculations); 
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);
        var result = await repository.GetByNameAsync("Does Not Exist");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCourseCodeAsync_WhenCohortExists_ReturnsCohortAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);
        var result = await repository.GetByCourseCodeAsync("COURSE-A");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Cohort A", result.Name);
    }

    [Fact]
    public async Task GetByCourseCodeAsync_WhenCohortDoesNotExist_ReturnsNullAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.GetByCourseCodeAsync("DOES-NOT-EXIST");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCohortsAsync_ReturnsAllCohortsAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.GetCohortsAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, X => X.Name == "Cohort A");
        Assert.Contains(result, X => X.Name == "Cohort B");
        Assert.Contains(result, X => X.Name == "Cohort C");
    }

    [Fact]
    public async Task GetCohortsAsync_WhenNoCohortsExist_ReturnsEmptyListAsync()
    {
        await using var context = CreateContext();

        var repository = CreateRepository(context);

        var result = await repository.GetCohortsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCohortTopRiskStudentsByComponentAsync_FiltersByComponentAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = (await repository.GetCohortTopRiskStudentsByComponentAsync(
            "poll-1",
            "Engagement",
            1,
            true,
            1,
            10)).ToList();

        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].StudentId);
        Assert.Equal("Alice", result[0].StudentName);
        Assert.Equal(4, result[0].RiskSum);

        Assert.Equal(2, result[1].StudentId);
        Assert.Equal("Bob", result[1].StudentName);
        Assert.Equal(2, result[1].RiskSum);
    }

    [Fact]
    public async Task GetCohortTopRiskStudentsByComponentAsync_FiltersLastVersionAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = (await repository.GetCohortTopRiskStudentsByComponentAsync(
            "poll-1",
            "Engagement",
            1,
            false,
            1,
            10)).ToList();

        Assert.Equal(2, result.Count);

        Assert.Contains(result, X => X.StudentId == 1);
        Assert.Contains(result, X => X.StudentId == 4);

        Assert.DoesNotContain(result, X => X.StudentId == 2);
    }

    [Fact]
    public async Task GetCohortTopRiskStudentsAsync_ReturnsStudentsOrderedByRiskDescendingAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = (await repository.GetCohortTopRiskStudentsAsync(
            "poll-1",
            1,
            true,
            1,
            10)).ToList();

        Assert.Equal(3, result.Count);

        Assert.Equal(1, result[0].StudentId);
        Assert.Equal(4, result[0].RiskSum);

        Assert.Equal(2, result[1].StudentId);
        Assert.Equal(3, result[1].RiskSum);
    }

    [Fact]
    public async Task GetCohortTopRiskStudentsAsync_AppliesPaginationAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var page1 = (await repository.GetCohortTopRiskStudentsAsync(
            "poll-1",
            1,
            true,
            1,
            1)).ToList();

        var page2 = (await repository.GetCohortTopRiskStudentsAsync(
            "poll-1",
            1,
            true,
            2,
            1)).ToList();

        Assert.Single(page1);
        Assert.Single(page2);

        Assert.Equal(1, page1[0].StudentId);
        Assert.Equal(2, page2[0].StudentId);
    }

    [Fact]
    public async Task CountStudentsAsync_ReturnsNumberOfDistinctStudentsAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.CountStudentsAsync("poll-1", 1, true);

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task CountStudentsAsync_WhenComponentSpecified_CountsOnlyThatComponentAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.CountStudentsAsync("poll-1", 1, true, "Engagement");

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CountStudentsAsync_WhenNoStudentsExist_ReturnsZeroAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.CountStudentsAsync(
            "poll-1",
            999,
            true);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetCohortsByPollUuidAsync_WhenLastVersionTrue_ReturnsOnlyLastVersionCohortsAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.GetCohortsByPollUuidAsync("poll-1", true);

        Assert.Equal(5, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Cohort A", result[0].Name);
    }

    [Fact]
    public async Task GetCohortsByPollUuidAsync_WhenLastVersionFalse_ReturnsPreviousVersionCohortsAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.GetCohortsByPollUuidAsync("poll-1", false);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Cohort A", result[0].Name);
    }

    [Fact]
    public async Task GetCohortsByPollIdAsync_ReturnsCohortsForPollAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.GetCohortsByPollIdAsync(1);

        Assert.Equal(7, result.Count);

        Assert.Contains(result, X => X.Name == "Cohort A");
        Assert.Contains(result, X => X.Name == "Cohort B");
    }

    [Fact]
    public async Task GetCohortsByPollIdAsync_DoesNotReturnCohortsFromAnotherPollAsync()
    {
        await using var context = CreateContext();
        await SeedAsync(context);
        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        var repository = CreateRepository(context);

        var result = await repository.GetCohortsByPollIdAsync(1);

        Assert.DoesNotContain(result, X => X.Name == "Cohort C");
    }
}
