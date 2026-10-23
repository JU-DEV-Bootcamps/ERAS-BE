using Eras.Application.Services;
using Eras.Domain.Entities;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Services;

public class EvaluationStatusServiceTests
{
    [Fact]
    public void ComputeStatus_ShouldReturnPending_WhenPollsAreNull()
    {
        // Arrange
        var evaluation = new Evaluation() 
        { 
            Polls = null!
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Pending, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnPending_WhenPollsAreEmpty()
    {
        // Arrange
        var evaluation = new Evaluation 
        { 
            Polls = new List<Poll>()
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Pending, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnReady_WhenThereAreNoAnswers_AndEvaluationHasNotEnded()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = new List<PollInstance>(),
            EndDate = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Ready, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnUncompleted_WhenThereAreNoAnswers_AndEvaluationHasEnded()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = new List<PollInstance>(),
            EndDate = DateTime.UtcNow.AddHours(-1)
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Uncompleted, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnInProgress_WhenPollInstanceHasAnswers_AndEvaluationHasNotEnded()
    {
        // Arrange
        var pollInstance = new PollInstance
        {
            Answers = new List<Answer>()
            {
                new Answer
                {
                    AnswerText = "example"
                }
            },
            SourcePollInstanceId = 1,
        };

        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = new List<PollInstance>() { pollInstance },
            EndDate = DateTime.UtcNow.AddHours(1),
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.InProgress, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnCompleted_WhenPollInstanceHasAnswers_AndEvaluationHasEnded()
    {
        // Arrange
        var pollInstance = new PollInstance
        {
            Answers = new List<Answer>()
            {
                new Answer
                {
                    AnswerText = "example"
                }
            },
            SourcePollInstanceId = null,
        };

        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = new List<PollInstance>() { pollInstance },
            EndDate = DateTime.UtcNow.AddHours(-1),
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Completed, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnInProgress_WhenPollInstanceHasSourcePollInstanceId()
    {
        // Arrange
        var pollInstance = new PollInstance
        {
            Answers = new List<Answer>()
            {
                new Answer
                {
                    AnswerText = "example"
                }
            },
            SourcePollInstanceId = 123,
        };

        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = new List<PollInstance>() { pollInstance },
            EndDate = DateTime.UtcNow.AddHours(1),
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);
        
        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.InProgress, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnCompleted_WhenPollInstanceHasSourcePollInstanceId_AndEvaluationHasEnded()
    {
        // Arrange
        var pollInstance = new PollInstance
        {
            Answers = new List<Answer>()
            {
                new Answer
                {
                    AnswerText = "example"
                }
            },
            SourcePollInstanceId = 123,
        };

        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = new List<PollInstance>() { pollInstance },
            EndDate = DateTime.UtcNow.AddHours(-1),
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Completed, result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnReady_WhenPollInstancesAreNull_AndEvaluationHasNotEnded()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = null!,
            EndDate = DateTime.UtcNow.AddHours(1),
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Ready,result);
    }

    [Fact]
    public void ComputeStatus_ShouldReturnUncompleted_WhenPollInstancesAreNull_AndEvaluationHasEnded()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            Polls = new List<Poll>()
            {
                new Poll
                {
                    Name = "eval"
                }
            },
            PollInstances = null!,
            EndDate = DateTime.UtcNow.AddHours(-1),
        };

        // Act
        var result = EvaluationStatusService.ComputeStatus(evaluation);

        // Assert
        Assert.Equal(EvaluationConstants.EvaluationStatus.Uncompleted, result);
    }
}
