using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Application.Models.Consolidator;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Consolidator.Queries;

public class PollAvgHandlerTest
{
    private readonly Mock<ILogger<PollAvgHandler>> _loggerMock;
    private readonly Mock<IPollInstanceRepository> _pollInstanceRepositoryMock;
    private readonly Mock<IEvaluationRepository> _evaluationRepositoryMock;

    private readonly PollAvgHandler _handler;

    public PollAvgHandlerTest()
    {
        _loggerMock = new Mock<ILogger<PollAvgHandler>>();
        _pollInstanceRepositoryMock = new Mock<IPollInstanceRepository>();
        _evaluationRepositoryMock = new Mock<IEvaluationRepository>();
        _handler = new PollAvgHandler(
            _loggerMock.Object,
            _pollInstanceRepositoryMock.Object,
            _evaluationRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEvaluationExistsAndReportIsReturned()
    {
        // Arrange
        var evaluationId = 123;
        var pollUuid = Guid.NewGuid();
        var cohortIds = new List<int> { 1, 2 };

        var startDate = new DateTime(2026, 1, 10, 8, 30, 0);
        var endDate = new DateTime(2026, 1, 20, 15, 45, 0);

        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var expectedResult = new AvgReportResponseVm();

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _pollInstanceRepositoryMock
            .Setup(x => x.GetReportByPollCohortAsync(
                pollUuid.ToString(),
                cohortIds,
                true,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(expectedResult);

        var request = new PollAvgQuery
        {
            EvaluationId = evaluationId,
            PollUuid = pollUuid,
            CohortIds = cohortIds,
            LastVersion = true
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Same(expectedResult, result.Body);

        _evaluationRepositoryMock.Verify(x => x.GetByIdAsync(evaluationId), Times.Once);

        _pollInstanceRepositoryMock.Verify(
            x => x.GetReportByPollCohortAsync(
                pollUuid.ToString(),
                cohortIds,
                true,
                It.Is<DateTime>(d =>
                    d == DateTime.SpecifyKind(startDate, DateTimeKind.Utc)),
                It.Is<DateTime>(d =>
                    d == DateTime.SpecifyKind(endDate, DateTimeKind.Utc).Date.AddDays(1).AddTicks(-1))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEvaluationDoesNotExist()
    {
        // Arrange
        var evaluationId = 123;

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync((Evaluation?)null);

        var request = new PollAvgQuery
        {
            EvaluationId = evaluationId,
            PollUuid = Guid.NewGuid(),
            CohortIds = new List<int>(),
            LastVersion = true
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal($"Failed: Evaluation with ID {evaluationId} not found.", result.Message);
        Assert.NotNull(result.Body);

        _pollInstanceRepositoryMock.Verify(
            x => x.GetReportByPollCohortAsync(
                It.IsAny<string>(),
                It.IsAny<List<int>>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenReportRepositoryThrowsException()
    {
        // Arrange
        var evaluationId = 123;
        var pollUuid = Guid.NewGuid();
        var exception = new Exception("Database error");

        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2026, 1, 10),
            EndDate = new DateTime(2026, 1, 20)
        };

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _pollInstanceRepositoryMock
            .Setup(x => x.GetReportByPollCohortAsync(
                It.IsAny<string>(),
                It.IsAny<List<int>>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ThrowsAsync(exception);

        var request = new PollAvgQuery
        {
            EvaluationId = evaluationId,
            PollUuid = pollUuid,
            CohortIds = new List<int> { 1 },
            LastVersion = false
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Failed: Database error", result.Message);
        Assert.NotNull(result.Body);
    }
}
