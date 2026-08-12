using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class PollVariableRepositoryTest
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

    private static List<ErasCalculationsByPollEntity> CreateErasCalculations()
    {
        return [
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-uuid",
                PollVariableId = 100,
                ComponentName = "Component 1",
                Question = "Question 1",
                Position = 1,
                AnswerText = "Yes",
                PollInstanceId = 1,
                StudentName = "Student 1",
                StudentEmail = "student@test.com",
                StudentId = 1,
                CohortId = 1,
                AnswerRisk = 1,
                PollInstanceRiskSum = 1,
                PollInstanceAnswersCount = 1,
                ComponentAverageRisk = 1,
                VariableAverageRisk = 1,
                AnswerCount = 1,
                AnswerPercentage = 100,
                CohortName = "Cohort A",
            },
            new ErasCalculationsByPollEntity
            {
                PollUuid = "poll-uuid",
                PollVariableId = 101,
                ComponentName = "Component 2",
                Question = "Question 2",
                Position = 2,
                AnswerText = "No",
                PollInstanceId = 2,
                StudentName = "Student 2",
                StudentEmail = "student2@test.com",
                StudentId = 2,
                CohortId = 1,
                AnswerRisk = 2,
                PollInstanceRiskSum = 2,
                PollInstanceAnswersCount = 1,
                ComponentAverageRisk = 2,
                VariableAverageRisk = 2,
                AnswerCount = 1,
                AnswerPercentage = 100,
                CohortName = "Cohort A",
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

    [Fact]
    public async Task GetByPollIdAndVariableIdAsync_ShouldReturnVariable_WhenExistsAsync()
    {
        await using var context = CreateContext();

        var variable = new VariableEntity{ Id = 10 };

        context.Variables.Add(variable);

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 1,
            PollId = 5,
            VariableId = 10,
            Variable = variable
        });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetByPollIdAndVariableIdAsync(5, 10);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task GetByPollIdAndVariableIdAsync_ShouldReturnNull_WhenDoesNotExistAsync()
    {
        await using var context = CreateContext();

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 1,
            PollId = 5,
            VariableId = 10
        });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetByPollIdAndVariableIdAsync(5, 999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByPollUuidVariableIdAsync_ShouldReturnEmpty_WhenPollDoesNotExistAsync()
    {
        await using var context = CreateContext();

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 100,
            PollId = 1,
            VariableId = 10
        });

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var pagination = new Pagination
        {
            Page = 1,
            PageSize = 10
        };

        var result = await repository.GetByPollUuidVariableIdAsync(
            "unknown-poll",
            [10],
            pagination);

        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetByPollUuidVariableIdAsync_ShouldReturnEmpty_WhenVariableIdsDoNotMatchAsync()
    {
        await using var context = CreateContext();

        context.Polls.Add(new PollEntity
        {
            Id = 1,
            Uuid = "poll-uuid"
        });

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 100,
            PollId = 1,
            VariableId = 10
        });

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var pagination = new Pagination
        {
            Page = 1,
            PageSize = 10
        };

        var result = await repository.GetByPollUuidVariableIdAsync(
            "poll-uuid",
            [999],
            pagination);

        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetByPollUuidAsync_ShouldReturnAnswersForRequestedPollAndVariablesAsync()
    {
        await using var context = CreateContext();

        var student = new StudentEntity
        {
            Id = 1,
            Uuid = "student-uuid",
            Name = "Student 1",
            Email = "student@test.com"
        };

        var variable = new VariableEntity
        {
            Id = 10
        };

        var component = new ComponentEntity
        {
            Id = 1
        };

        variable.ComponentId = component.Id;
        variable.Component = component;

        context.Students.Add(student);
        context.Variables.Add(variable);
        context.Components.Add(component);

        context.PollInstances.Add(new PollInstanceEntity
        {
            Id = 1,
            Uuid = "poll-instance-uuid",
            StudentId = student.Id,
            Student = student
        });

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 100,
            VariableId = variable.Id,
            Variable = variable
        });

        context.Answers.Add(new AnswerEntity
        {
            Id = 1,
            PollInstanceId = 1,
            PollVariableId = 100,
            RiskLevel = 2
        });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetByPollUuidAsync(
            "poll-instance-uuid",
            "10");

        Assert.Single(result);
        Assert.Equal(1, result[0].Answer.Id);
        Assert.Equal(10, result[0].Variable.Id);
        Assert.Equal(1, result[0].Student.Id);
    }

    [Fact]
    public async Task GetByPollUuidAsync_ShouldGroupAnswersByStudentAsync()
    {
        await using var context = CreateContext();

        var student = new StudentEntity
        {
            Id = 1,
            Uuid = "student-uuid",
            Name = "Student 1",
            Email = "student@test.com"
        };

        var component = new ComponentEntity
        {
            Id = 1
        };

        var variable1 = new VariableEntity
        {
            Id = 10,
            ComponentId = 1,
            Component = component
        };

        var variable2 = new VariableEntity
        {
            Id = 20,
            ComponentId = 1,
            Component = component
        };

        context.Students.Add(student);
        context.Components.Add(component);
        context.Variables.AddRange(variable1, variable2);

        context.PollInstances.Add(new PollInstanceEntity
        {
            Id = 1,
            Uuid = "poll-instance-uuid",
            StudentId = 1,
            Student = student
        });

        context.PollVariables.AddRange(
            new PollVariableJoin
            {
                Id = 100,
                VariableId = 10,
                Variable = variable1
            },
            new PollVariableJoin
            {
                Id = 200,
                VariableId = 20,
                Variable = variable2
            });

        context.Answers.AddRange(
            new AnswerEntity
            {
                Id = 1,
                PollInstanceId = 1,
                PollVariableId = 100,
                RiskLevel = 1
            },
            new AnswerEntity
            {
                Id = 2,
                PollInstanceId = 1,
                PollVariableId = 200,
                RiskLevel = 3
            });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetByPollUuidAsync(
            "poll-instance-uuid",
            "10,20");

        // The two answers belong to the same student, therefore
        // GroupBy(Student.Name) produces one result.
        Assert.Single(result);
        Assert.Equal(1, result[0].Student.Id);
    }

    [Fact]
    public async Task GetByPollUuidAsync_ShouldReturnEmpty_WhenPollDoesNotExistAsync()
    {
        await using var context = CreateContext();

        var result = await new PollVariableRepository(context)
            .GetByPollUuidAsync("unknown-poll", "1");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAnswersByPollUuidAsync_ShouldReturnAnswersAsync()
    {
        await using var context = CreateContext();

        context.PollInstances.AddRange(
            new PollInstanceEntity
            {
                Id = 1,
                Uuid = "poll-uuid"
            },
            new PollInstanceEntity
            {
                Id = 2,
                Uuid = "other-poll"
            });

        context.Answers.AddRange(
            new AnswerEntity
            {
                Id = 1,
                PollInstanceId = 1,
                AnswerText = "Answer 1"
            },
            new AnswerEntity
            {
                Id = 2,
                PollInstanceId = 2,
                AnswerText = "Answer 2"
            });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetAnswersByPollUuidAsync("poll-uuid");

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Answer 1", result[0].AnswerText);
    }

    [Fact]
    public async Task GetAnswersByPollUuidAsync_ShouldReturnEmpty_WhenPollDoesNotExistAsync()
    {
        await using var context = CreateContext();

        context.PollInstances.Add(new PollInstanceEntity
        {
            Id = 1,
            Uuid = "poll-uuid"
        });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetAnswersByPollUuidAsync("unknown-poll");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllWithVariablesAsync_ShouldReturnPollVariablesAsync()
    {
        await using var context = CreateContext();

        var variable = new VariableEntity
        {
            Id = 10
        };

        context.Variables.Add(variable);

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 1,
            PollId = 5,
            VariableId = 10,
            Variable = variable
        });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetAllWithVariablesAsync();

        Assert.Single(result);
        Assert.Equal(10, result[0].Id);
    }

    [Fact]
    public async Task GetAllWithVariablesAsync_ShouldReturnEmpty_WhenNoPollVariablesExistAsync()
    {
        await using var context = CreateContext();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetAllWithVariablesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllWithVariablesByPollIdAsync_ShouldReturnOnlyVariablesForPollAsync()
    {
        await using var context = CreateContext();

        var variable1 = new VariableEntity
        {
            Id = 10
        };

        var variable2 = new VariableEntity
        {
            Id = 20
        };

        context.Variables.AddRange(variable1, variable2);

        context.PollVariables.AddRange(
            new PollVariableJoin
            {
                Id = 1,
                PollId = 5,
                VariableId = 10,
                Variable = variable1
            },
            new PollVariableJoin
            {
                Id = 2,
                PollId = 6,
                VariableId = 20,
                Variable = variable2
            });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetAllWithVariablesByPollIdAsync(5);

        Assert.Single(result);
        Assert.Equal(10, result[0].Id);
    }

    [Fact]
    public async Task GetAllWithVariablesByPollIdAsync_ShouldReturnEmpty_WhenPollHasNoVariablesAsync()
    {
        await using var context = CreateContext();

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 1,
            PollId = 5,
            VariableId = 10
        });

        await context.SaveChangesAsync();

        var repository = new PollVariableRepository(context);

        var result = await repository.GetAllWithVariablesByPollIdAsync(999);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddBatchPollVariablesAsync_ShouldThrow_WhenSaveChangesFailsAsync()
    {
        await using var context = CreateContext();

        var repository = new PollVariableRepository(context);
        var variables = new List<Variable>
        {
            new Variable()
        };

        await Assert.ThrowsAnyAsync<Exception>(
            () => repository.AddBatchPollVariablesAsync(variables));
    }
}

