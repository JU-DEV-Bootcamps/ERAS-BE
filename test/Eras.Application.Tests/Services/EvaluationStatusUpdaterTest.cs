using Eras.Application.Contracts.Persistence;
using Eras.Application.Services;
using Eras.Domain.Entities;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Services;

public class EvaluationStatusUpdaterTests
{
    private readonly Mock<IEvaluationRepository> _repositoryMock;
    private readonly EvaluationStatusUpdater _sut;

    public EvaluationStatusUpdaterTests()
    {
        _repositoryMock = new Mock<IEvaluationRepository>();
        _sut = new EvaluationStatusUpdater(_repositoryMock.Object);
    }

    [Fact]
    public async Task UpdateStatusAsync_ById_ShouldDoNothing_WhenEvaluationDoesNotExist()
    {
        // Arrange
        const int evaluationId = 123;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync((Evaluation?)null);

        // Act
        await _sut.UpdateStatusAsync(evaluationId);

        // Assert
        _repositoryMock.Verify(x => x.GetByIdAsync(evaluationId), Times.Once);

        _repositoryMock.Verify(
            x => x.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ById_ShouldUpdateStatus_WhenStatusHasChanged()
    {
        // Arrange
        const int evaluationId = 123;

        var evaluation = CreateEvaluation(
            evaluationId,
            status: "Pending",
            endDate: DateTime.UtcNow.AddHours(1));

        _repositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _sut.UpdateStatusAsync(evaluationId);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Equal("Ready", evaluation.Status);
      
        _repositoryMock.Verify(x => x.GetByIdAsync(evaluationId), Times.Once);

        _repositoryMock.Verify(
            x => x.UpdateStatusAsync(evaluationId, "Ready"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ById_ShouldNotUpdate_WhenStatusHasNotChanged()
    {
        // Arrange
        const int evaluationId = 123;

        var originalModifiedAt = DateTime.UtcNow.AddHours(-1);

        var evaluation = CreateEvaluation(
            evaluationId,
            status: "Ready",
            endDate: DateTime.UtcNow.AddHours(1));

        evaluation.Audit.ModifiedAt = originalModifiedAt;

        _repositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        // Act
        await _sut.UpdateStatusAsync(evaluationId);

        // Assert
        Assert.Equal("Ready", evaluation.Status);
        Assert.Equal(originalModifiedAt, evaluation.Audit.ModifiedAt);

        _repositoryMock.Verify(x => x.GetByIdAsync(evaluationId), Times.Once);

        _repositoryMock.Verify(
            x => x.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ByEvaluation_ShouldUpdateStatus_WhenStatusHasChanged()
    {
        // Arrange
        const int evaluationId = 456;

        var evaluation = CreateEvaluation(evaluationId, "Pending", DateTime.UtcNow.AddHours(1));

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _sut.UpdateStatusAsync(evaluation);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.Equal("Ready", evaluation.Status);

        _repositoryMock.Verify(
            x => x.UpdateStatusAsync(evaluationId, "Ready"), Times.Once);

        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ByEvaluation_ShouldNotUpdate_WhenStatusHasNotChanged()
    {
        // Arrange
        const int evaluationId = 456;

        var originalModifiedAt = DateTime.UtcNow.AddHours(-1);

        var evaluation = CreateEvaluation(evaluationId,  "Ready", DateTime.UtcNow.AddHours(1));

        evaluation.Audit.ModifiedAt = originalModifiedAt;

        // Act
        await _sut.UpdateStatusAsync(evaluation);

        // Assert
        Assert.Equal("Ready", evaluation.Status);
        Assert.Equal(originalModifiedAt, evaluation.Audit.ModifiedAt);

        _repositoryMock.Verify(
            x => x.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldSetCompleted_WhenEvaluationHasAnswersAndHasEnded()
    {
        // Arrange
        const int evaluationId = 789;

        var evaluation = CreateEvaluation(evaluationId, "InProgress", DateTime.UtcNow.AddHours(-1));

        evaluation.PollInstances = new List<PollInstance>
        {
            new PollInstance
            {
                Answers = new List<Answer>
                {
                    new Answer()
                }
            }
        };

        // Act
        await _sut.UpdateStatusAsync(evaluation);

        // Assert
        Assert.Equal("Completed", evaluation.Status);

        _repositoryMock.Verify(
            x => x.UpdateStatusAsync(evaluationId, "Completed"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldSetUncompleted_WhenEvaluationHasNoAnswersAndHasEnded()
    {
        // Arrange
        const int evaluationId = 999;

        var evaluation = CreateEvaluation(evaluationId, "Ready", DateTime.UtcNow.AddHours(-1));

        // Act
        await _sut.UpdateStatusAsync(evaluation);

        // Assert
        Assert.Equal("Uncompleted", evaluation.Status);

        _repositoryMock.Verify(
            x => x.UpdateStatusAsync(evaluationId, "Uncompleted"), Times.Once);
    }

    private static Evaluation CreateEvaluation(int id, string status, DateTime endDate)
    {
        return new Evaluation
        {
            Id = id,
            Status = status,
            EndDate = endDate,
            Polls = new List<Poll>
            {
                new Poll()
            },
            PollInstances = new List<PollInstance>(),
            Audit = new Domain.Common.AuditInfo()
        };
    }
}