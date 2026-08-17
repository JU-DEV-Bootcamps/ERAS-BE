using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class PollCohortRepositoryTests : RepositoryTestBase
{

    private static PollCohortRepository CreateRepository(AppDbContext Context)
    {
        return new PollCohortRepository(Context);
    }

    private static Mock<DbSet<ErasCalculationsByPollEntity>> CreateMockErasCalculations(
        IEnumerable<ErasCalculationsByPollEntity> Data)
    {
        var queryable = new TestAsyncEnumerable<ErasCalculationsByPollEntity>(Data);

        var dbSet = new Mock<DbSet<ErasCalculationsByPollEntity>>();

        dbSet
            .As<IAsyncEnumerable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
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

    private static List<ErasCalculationsByPollEntity> CreateErasCalculations()
    {
        return
        [
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 1,
                CohortName = "Cohort A",
                PollInstanceId = 101,
                StudentName = "Alice",
                ComponentName = "Engagement",
                AverageRiskByCohortComponent = 3,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com"
            },

            // Duplicate logical result to verify Distinct()
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 1,
                CohortName = "Cohort A",
                PollInstanceId = 101,
                StudentName = "Alice",
                ComponentName = "Engagement",
                AverageRiskByCohortComponent = 3,
                Question = "Question2",
                AnswerText = "text5",
                StudentEmail = "ali@mail.com"
            },

            // Last version - Cohort B / Attendance
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 2,
                CohortId = 2,
                CohortName = "Cohort B",
                PollInstanceId = 102,
                StudentName = "Bob",
                ComponentName = "Attendance",
                AverageRiskByCohortComponent = 4,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "bob@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 1,
                CohortId = 1,
                CohortName = "Cohort A",
                PollInstanceId = 91,
                StudentName = "Alice",
                ComponentName = "Engagement",
                AverageRiskByCohortComponent = 2,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-1",
                PollId = 1,
                PollVersion = 1,
                CohortId = 2,
                CohortName = "Cohort B",
                PollInstanceId = 92,
                StudentName = "Bob",
                ComponentName = "Attendance",
                AverageRiskByCohortComponent = 1,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "bob@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-2",
                PollId = 2,
                PollVersion = 1,
                CohortId = 1,
                CohortName = "Cohort A",
                PollInstanceId = 201,
                StudentName = "Charlie",
                ComponentName = "Engagement",
                AverageRiskByCohortComponent = 5,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "charl@mail.com"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-risk",
                PollId = 3,
                PollVersion = 1,
                CohortId = 1,
                CohortName = "Cohort A",
                PollInstanceId = 301,
                StudentName = "Alice",
                PollInstanceRiskSum = 10,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com",
                ComponentName = "Section"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-risk",
                PollId = 3,
                PollVersion = 1,
                CohortId = 1,
                CohortName = "Cohort A",
                PollInstanceId = 301,
                StudentName = "Alice",
                PollInstanceRiskSum = 10,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com",
                ComponentName = "Third"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-risk",
                PollId = 3,
                PollVersion = 1,
                CohortId = 1,
                CohortName = "Cohort A",
                PollInstanceId = 302,
                StudentName = "Bob",
                PollInstanceRiskSum = 7,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com",
                ComponentName = "Academic"
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-risk",
                PollId = 3,
                PollVersion = 1,
                CohortId = 2,
                CohortName = "Cohort B",
                PollInstanceId = 303,
                StudentName = "Charlie",
                PollInstanceRiskSum = 20,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com",
                ComponentName = "Individual"
            }
        ];
    }

    private static async Task SeedPollDataAsync(AppDbContext Context)
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
            });

        Context.Students.AddRange(
            new StudentEntity
            {
                Id = 1,
                Email = "Alice@mail.com",
                Name = "alice",
                Uuid = "123"
            },
            new StudentEntity
            {
                Id = 2,
                Email = "Bob@mail.com",
                Name = "Bob",
                Uuid = "1234"
            });

        Context.StudentCohorts.AddRange(
            new StudentCohortJoin
            {
                Id = 1,
                StudentId = 1,
                CohortId = 1,
                //Student = new StudentEntity()
                //{
                //    Id = 1,
                //    Email = "stu@mail.com",
                //    Name = "Alice",
                //    Uuid = "123"
                //}
            },
            new StudentCohortJoin
            {
                Id = 2,
                StudentId = 2,
                CohortId = 2,
                //Student = new StudentEntity()
                //{
                //    Id = 2,
                //    Email = "stu2@mail.com",
                //    Name = "Bob",
                //    Uuid = "1234"
                //}
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
                LastVersion = 1
            });

        Context.PollInstances.AddRange(
            new PollInstanceEntity
            {
                Id = 101,
                StudentId = 1
            },
            new PollInstanceEntity
            {
                Id = 102,
                StudentId = 2
            });

        Context.Variables.AddRange(
            new VariableEntity
            {
                Id = 1,
                Name = "Variable 1"
            },
            new VariableEntity
            {
                Id = 2,
                Name = "Variable 2"
            });

        Context.PollVariables.AddRange(
            new PollVariableJoin
            {
                Id = 1,
                PollId = 1,
                VariableId = 1
            },
            new PollVariableJoin
            {
                Id = 2,
                PollId = 1,
                VariableId = 2
            });

        Context.Answers.AddRange(
            new AnswerEntity
            {
                Id = 1,
                PollInstanceId = 101,
                PollVariableId = 1,
                AnswerText = "Yes",
                RiskLevel = 1
            },
            new AnswerEntity
            {
                Id = 2,
                PollInstanceId = 101,
                PollVariableId = 2,
                AnswerText = "No",
                RiskLevel = 2
            },
            new AnswerEntity
            {
                Id = 3,
                PollInstanceId = 102,
                PollVariableId = 1,
                AnswerText = "Yes",
                RiskLevel = 3
            });

        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetPollsByCohortIdAsync_WhenCohortHasPolls_ReturnsDistinctPollsAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetPollsByCohortIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task GetPollsByCohortIdAsync_WhenCohortDoesNotExist_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetPollsByCohortIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPollsByCohortIdAsync_WhenCohortHasNoStudents_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        context.Cohorts.Add(new CohortEntity
        {
            Id = 10,
            Name = "Empty Cohort",
            CourseCode = "EMPTY"
        });

        await context.SaveChangesAsync();

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetPollsByCohortIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPollVariablesAsync_WhenPollAndCohortHaveAnswers_ReturnsVariablesAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetPollVariablesAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, X =>X.PollId == 1 && X.VariableId == 1 && X.VariableName == "Variable 1");

        Assert.Contains(
            result,
            X => X.PollId == 1 && X.VariableId == 2 && X.VariableName == "Variable 2");
    }

    [Fact]
    public async Task GetPollVariablesAsync_WhenCohortDoesNotMatch_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetPollVariablesAsync(1, 3);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPollVariablesAsync_WhenPollDoesNotExist_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetPollVariablesAsync(999, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetCohortComponentsByPoll_WhenLastVersionIsTrue_ReturnsOnlyLastVersionAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var calculations = CreateErasCalculations();
        context.ErasCalculationsByPoll =
            CreateMockErasCalculations(calculations).Object;

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetCohortComponentsByPoll("poll-1", true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Contains(result,
            X => X.CohortId == 1
                && X.CohortName == "Cohort A"
                && X.ComponentName == "Engagement"
                && X.AverageRiskByCohortComponent == 3);

        Assert.Contains(result,
            X => X.CohortId == 2
                && X.CohortName == "Cohort B"
                && X.ComponentName == "Attendance"
                && X.AverageRiskByCohortComponent == 4);
    }

    [Fact]
    public async Task GetCohortComponentsByPoll_WhenPollDoesNotExist_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);
        var calculations = CreateErasCalculations();
        context.ErasCalculationsByPoll = CreateMockErasCalculations(calculations).Object;
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetCohortComponentsByPoll("does-not-exist",true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCohortComponentsByPoll_WhenNoCalculationsExist_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        context.ErasCalculationsByPoll =
            CreateMockErasCalculations(
                Enumerable.Empty<ErasCalculationsByPollEntity>()
            ).Object;

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetCohortComponentsByPoll("poll-1", true);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCohortStudentsRiskByPoll_ReturnsStudentsOrderedByRiskAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var calculations = CreateErasCalculations();
        context.ErasCalculationsByPoll =
            CreateMockErasCalculations(calculations).Object;

        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetCohortStudentsRiskByPoll("poll-risk", 1);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(301, result[0].PollInstanceId);
        Assert.Equal("Alice", result[0].StudentName);
        Assert.Equal(10, result[0].PollInstanceRiskSum);

        Assert.Equal(302, result[1].PollInstanceId);
        Assert.Equal("Bob", result[1].StudentName);
        Assert.Equal(7, result[1].PollInstanceRiskSum);
    }

    [Fact]
    public async Task GetCohortStudentsRiskByPoll_DistinctByPollInstanceId_RemovesDuplicatesAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var calculations = new List<ErasCalculationsByPollEntity>
        {
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-3",
                PollVersion = 1,
                CohortId = 1,
                PollInstanceId = 500,
                StudentName = "Alice",
                PollInstanceRiskSum = 15,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com",
                CohortName = "Cohort",
                ComponentName = "academic",
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-3",
                PollVersion = 1,
                CohortId = 1,
                PollInstanceId = 500,
                StudentName = "Alice",
                PollInstanceRiskSum = 15,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "ali@mail.com",
                CohortName = "Cohort",
                ComponentName = "academic",
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-3",
                PollVersion = 1,
                CohortId = 1,
                PollInstanceId = 501,
                StudentName = "Bob",
                PollInstanceRiskSum = 10,
                Question = "Question",
                AnswerText = "text",
                StudentEmail = "bob@mail.com",
                CohortName = "Cohort",
                ComponentName = "academic",
            }
        };
        context.ErasCalculationsByPoll = CreateMockErasCalculations(calculations).Object;
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetCohortStudentsRiskByPoll("poll-3", 1);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(500, result[0].PollInstanceId);
        Assert.Equal(501, result[1].PollInstanceId);
    }

    [Fact]
    public async Task GetCohortStudentsRiskByPoll_WhenCohortDoesNotMatch_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var calculations = CreateErasCalculations();
        context.ErasCalculationsByPoll = CreateMockErasCalculations(calculations).Object;
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetCohortStudentsRiskByPoll("poll-risk", 999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCohortStudentsRiskByPoll_WhenPollDoesNotMatch_ReturnsEmptyListAsync()
    {
        // Arrange
        await using var context = CreateContext();
        await SeedPollDataAsync(context);

        var calculations = CreateErasCalculations();
        context.ErasCalculationsByPoll = CreateMockErasCalculations(calculations).Object;
        var repository = CreateRepository(context);

        // Act
        var result = await repository.GetCohortStudentsRiskByPoll("does-not-exist", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
