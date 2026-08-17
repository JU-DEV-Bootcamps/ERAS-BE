using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

public class StudentAnswersRepositoryTest : RepositoryTestBase
{

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
                StudentId = 1,
                PollId = 10,
                Question = "Question 1",
                Position = 1,
                ComponentName = "Component 1",
                AnswerText = "Yes",
                AnswerRisk = 2,
                PollUuid = "10",
                CohortName = "Cohort A",
                StudentEmail = "A@mail.com",
                StudentName = "Anne"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                Question = "Question 2",
                Position = 2,
                ComponentName = "Component 2",
                AnswerText = "No",
                AnswerRisk = 3,
                PollUuid = "10",
                CohortName = "Cohort A",
                StudentEmail = "A@mail.com",
                StudentName = "Anne"
            }
        ];
    }

    [Fact]
    public async Task GetStudentAnswersPagedAsync_ShouldReturnStudentAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;
        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersPagedAsync(StudentId: 1, PollId: 10, Page: 1, PageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Items.Count);

        Assert.Equal("Question 1", result.Items[0].Variable);
        Assert.Equal(1, result.Items[0].Position);
        Assert.Equal("Component 1", result.Items[0].Component);
        Assert.Equal("Yes", result.Items[0].Answer);
        Assert.Equal(2, result.Items[0].Score);

        Assert.Equal("Question 2", result.Items[1].Variable);
        Assert.Equal(2, result.Items[1].Position);
        Assert.Equal("Component 2", result.Items[1].Component);
        Assert.Equal("No", result.Items[1].Answer);
        Assert.Equal(3, result.Items[1].Score);
    }

    [Fact]
    public async Task GetStudentAnswersPagedAsync_ShouldReturnEmpty_WhenStudentHasNoAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;
        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersPagedAsync(StudentId: 999, PollId: 10, Page: 1, PageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetStudentAnswersPagedAsync_ShouldReturnEmpty_WhenPollDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;
        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersPagedAsync(StudentId: 1, PollId: 999, Page: 1, PageSize: 10);

        // Assert
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetStudentAnswersPagedAsync_ShouldReturnDistinctQuestionsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;
        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersPagedAsync(StudentId: 1, PollId: 10, Page: 1, PageSize: 10);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetStudentAnswersPagedAsync_ShouldOrderByPositionAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;
        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersPagedAsync(StudentId: 1,PollId: 10, Page: 1, PageSize: 10);
 
        // Assert
        Assert.Equal(2, result.Items.Count);

        Assert.Equal(1, result.Items[0].Position);
        Assert.Equal(2, result.Items[1].Position);
    }

    [Fact]
    public async Task GetStudentAnswersPagedAsync_ShouldApplyPageSizeAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;
        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersPagedAsync(StudentId: 1, PollId: 10, Page: 1, PageSize: 2);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Items.Count);

        Assert.Equal("Question 1", result.Items[0].Variable);
        Assert.Equal("Question 2", result.Items[1].Variable);
    }

    [Fact]
    public async Task GetStudentAnswersPagedAsync_ShouldReturnEmpty_WhenPageIsBeyondResultsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var calculations = CreateErasCalculations();
        var calculationsDbSet = CreateMockErasCalculations(calculations);
        context.ErasCalculationsByPoll = calculationsDbSet.Object;
        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersPagedAsync(StudentId: 1, PollId: 10, Page: 2,PageSize: 10);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetStudentAnswersAsync_ShouldReturnMatchingAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var component = new ComponentEntity
        {
            Id = 1,
            Name = "Component 1"
        };

        var variable = new VariableEntity
        {
            Id = 10,
            Name = "Variable 1",
            ComponentId = 1,
            Component = component
        };

        var student = new StudentEntity
        {
            Id = 1,
            Name = "Student 1",
            Uuid = "student-1",
            Email = "st@mail.com"
        };

        var pollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Student = student
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 200,
            PollId = 10,
            VariableId = 10,
            Variable = variable
        };

        context.Components.Add(component);
        context.Variables.Add(variable);
        context.Students.Add(student);
        context.PollInstances.Add(pollInstance);
        context.PollVariables.Add(pollVariable);

        context.Answers.Add(new AnswerEntity
        {
            Id = 1,
            PollInstanceId = 100,
            PollVariableId = 200,
            AnswerText = "Yes",
            RiskLevel = 2
        });

        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersAsync(StudentId: 1, PollId: 10);

        // Assert
        Assert.Single(result);
        Assert.Equal("Variable 1", result[0].Variable);
        Assert.Equal(200, result[0].Position);
        Assert.Equal("Component 1", result[0].Component);
        Assert.Equal("Yes", result[0].Answer);
        Assert.Equal(2, result[0].Score);
    }

    [Fact]
    public async Task GetStudentAnswersAsync_ShouldReturnEmpty_WhenStudentDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.Answers.Add(new AnswerEntity
        {
            Id = 1,
            PollInstanceId = 100,
            PollVariableId = 200,
            AnswerText = "Yes"
        });

        context.PollInstances.Add(new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1
        });

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 200,
            PollId = 10,
            VariableId = 20
        });

        context.Variables.Add(new VariableEntity
        {
            Id = 20,
            Name = "Variable"
        });

        context.Components.Add(new ComponentEntity
        {
            Id = 1,
            Name = "Component"
        });

        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersAsync(StudentId: 999, PollId: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStudentAnswersAsync_ShouldReturnEmpty_WhenPollDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.PollInstances.Add(new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1
        });

        context.PollVariables.Add(new PollVariableJoin
        {
            Id = 200,
            PollId = 10,
            VariableId = 20
        });

        context.Variables.Add(new VariableEntity
        {
            Id = 20,
            Name = "Variable"
        });

        context.Components.Add(new ComponentEntity
        {
            Id = 1,
            Name = "Component"
        });

        context.Answers.Add(new AnswerEntity
        {
            Id = 1,
            PollInstanceId = 100,
            PollVariableId = 200,
            AnswerText = "Yes"
        });

        await context.SaveChangesAsync();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersAsync(StudentId: 1, PollId: 999);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStudentAnswersAsync_ShouldReturnEmpty_WhenNoAnswersExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new StudentAnswersRepository(context);

        // Act
        var result = await repository.GetStudentAnswersAsync(StudentId: 1, PollId: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotImplementedExceptionAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new StudentAnswersRepository(context);

        var entity = new StudentAnswer
        {
            Variable = "Variable",
            Position = 1,
            Component = "Component",
            Answer = "Answer",
            Score = 1
        };

        // Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => repository.UpdateAsync(entity));
    }
}