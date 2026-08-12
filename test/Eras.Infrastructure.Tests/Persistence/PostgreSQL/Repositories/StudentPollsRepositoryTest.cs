using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentPollsRepositoryTest
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldReturnPoll_WhenStudentHasAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        var pollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Uuid = "poll-instance-1"
        };

        var answer = new AnswerEntity
        {
            Id = 1000,
            PollInstanceId = 100,
            PollVariableId = 10,
            AnswerText = "Yes",
            PollInstance = pollInstance
        };

        pollVariable.Answers = new List<AnswerEntity> { answer };
        poll.PollVariables = new List<PollVariableJoin> { pollVariable };

        context.Polls.Add(poll);
        context.PollVariables.Add(pollVariable);
        context.PollInstances.Add(pollInstance);
        context.Answers.Add(answer);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        Assert.Equal(1, result[0].Id);
        Assert.Equal("poll-1", result[0].Uuid);
        Assert.Equal("Poll 1", result[0].Name);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldReturnEmpty_WhenStudentHasNoAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        context.Polls.Add(poll);
        context.PollVariables.Add(pollVariable);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldReturnEmpty_WhenDatabaseIsEmptyAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldReturnMultiplePolls_WhenStudentHasAnswersInMultiplePollsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll1 = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var poll2 = new PollEntity
        {
            Id = 2,
            Uuid = "poll-2",
            Name = "Poll 2"
        };

        var pollVariable1 = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        var pollVariable2 = new PollVariableJoin
        {
            Id = 20,
            PollId = 2
        };

        var pollInstance1 = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Uuid = "instance-1"
        };

        var pollInstance2 = new PollInstanceEntity
        {
            Id = 200,
            StudentId = 1,
            Uuid = "instance-2"
        };

        var answer1 = new AnswerEntity
        {
            Id = 1000,
            PollInstanceId = 100,
            PollVariableId = 10,
            AnswerText = "Answer 1"
        };

        var answer2 = new AnswerEntity
        {
            Id = 2000,
            PollInstanceId = 200,
            PollVariableId = 20,
            AnswerText = "Answer 2"
        };

        context.Polls.AddRange(poll1, poll2);
        context.PollVariables.AddRange(pollVariable1, pollVariable2);
        context.PollInstances.AddRange(pollInstance1, pollInstance2);
        context.Answers.AddRange(answer1, answer2);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(result, P => P.Id == 1);
        Assert.Contains(result, P => P.Id == 2);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldNotReturnPoll_WhenAnswerBelongsToAnotherStudentAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        var pollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 999,
            Uuid = "instance-1"
        };

        var answer = new AnswerEntity
        {
            Id = 1000,
            PollInstanceId = 100,
            PollVariableId = 10,
            AnswerText = "Other student's answer"
        };

        context.Polls.Add(poll);
        context.PollVariables.Add(pollVariable);
        context.PollInstances.Add(pollInstance);
        context.Answers.Add(answer);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldReturnPollOnlyOnce_WhenStudentHasMultipleAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        var pollInstance1 = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Uuid = "instance-1"
        };

        var pollInstance2 = new PollInstanceEntity
        {
            Id = 200,
            StudentId = 1,
            Uuid = "instance-2"
        };

        var answer1 = new AnswerEntity
        {
            Id = 1000,
            PollInstanceId = 100,
            PollVariableId = 10,
            AnswerText = "Yes"
        };

        var answer2 = new AnswerEntity
        {
            Id = 2000,
            PollInstanceId = 200,
            PollVariableId = 10,
            AnswerText = "No"
        };

        context.Polls.Add(poll);
        context.PollVariables.Add(pollVariable);
        context.PollInstances.AddRange(pollInstance1, pollInstance2);
        context.Answers.AddRange(answer1, answer2);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldIncludeOnlyMatchingStudentAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        var studentPollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Uuid = "student-1-instance"
        };

        var otherStudentPollInstance = new PollInstanceEntity
        {
            Id = 200,
            StudentId = 2,
            Uuid = "student-2-instance"
        };

        var studentAnswer = new AnswerEntity
        {
            Id = 1000,
            PollInstanceId = 100,
            PollVariableId = 10,
            AnswerText = "Student 1 answer"
        };

        var otherStudentAnswer = new AnswerEntity
        {
            Id = 2000,
            PollInstanceId = 200,
            PollVariableId = 10,
            AnswerText = "Student 2 answer"
        };

        context.Polls.Add(poll);
        context.PollVariables.Add(pollVariable);
        context.PollInstances.AddRange(
            studentPollInstance,
            otherStudentPollInstance);

        context.Answers.AddRange(
            studentAnswer,
            otherStudentAnswer);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.Single(result);

        var returnedPoll = result[0];
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldIncludeMultiplePollVariablesAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var variable1 = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        var variable2 = new PollVariableJoin
        {
            Id = 20,
            PollId = 1
        };

        var pollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Uuid = "instance-1"
        };

        var answer1 = new AnswerEntity
        {
            Id = 1000,
            PollInstanceId = 100,
            PollVariableId = 10,
            AnswerText = "Answer 1"
        };

        var answer2 = new AnswerEntity
        {
            Id = 2000,
            PollInstanceId = 100,
            PollVariableId = 20,
            AnswerText = "Answer 2"
        };

        context.Polls.Add(poll);
        context.PollVariables.AddRange(variable1, variable2);
        context.PollInstances.Add(pollInstance);
        context.Answers.AddRange(answer1, answer2);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.Single(result);

        var returnedPoll = result[0];

        Assert.Equal(0, returnedPoll.Components.Count);
    }

    [Fact]
    public async Task GetPollsByStudentIdAsync_ShouldIncludePollVariableWithEmptyAnswersAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var poll = new PollEntity
        {
            Id = 1,
            Uuid = "poll-1",
            Name = "Poll 1"
        };

        var answeredVariable = new PollVariableJoin
        {
            Id = 10,
            PollId = 1
        };

        var unansweredVariable = new PollVariableJoin
        {
            Id = 20,
            PollId = 1
        };

        var pollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Uuid = "instance-1"
        };

        var answer = new AnswerEntity
        {
            Id = 1000,
            PollInstanceId = 100,
            PollVariableId = 10,
            AnswerText = "Yes"
        };

        context.Polls.Add(poll);
        context.PollVariables.AddRange(
            answeredVariable,
            unansweredVariable);

        context.PollInstances.Add(pollInstance);
        context.Answers.Add(answer);

        await context.SaveChangesAsync();

        var repository = new StudentPollsRepository(context);

        // Act
        var result = await repository.GetPollsByStudentIdAsync(1);

        // Assert
        Assert.Single(result);

        var returnedPoll = result[0];

        Assert.Equal("Poll 1", returnedPoll.Name);
    }
}
