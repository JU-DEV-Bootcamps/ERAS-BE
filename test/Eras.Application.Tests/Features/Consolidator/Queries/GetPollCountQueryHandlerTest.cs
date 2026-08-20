using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Application.Models.Consolidator;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Consolidator.Queries;

public class PollCountQueryHandlerTests
{
    private readonly Mock<ILogger<PollCountQueryHandler>> _loggerMock;
    private readonly Mock<IPollInstanceRepository> _pollInstanceRepositoryMock;
    private readonly Mock<IEvaluationRepository> _evaluationRepositoryMock;

    private readonly PollCountQueryHandler _handler;

    public PollCountQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<PollCountQueryHandler>>();
        _pollInstanceRepositoryMock = new Mock<IPollInstanceRepository>();
        _evaluationRepositoryMock = new Mock<IEvaluationRepository>();

        _handler = new PollCountQueryHandler(
            _loggerMock.Object,
            _pollInstanceRepositoryMock.Object,
            _evaluationRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEvaluationAndReportExist()
    {
        // Arrange
        var evaluationId = 123;
        var pollUuid = Guid.NewGuid().ToString();
        var cohortIds = new List<int> { 1, 2 };
        var variableIds = new List<int> { 10, 20 };

        var startDate = new DateTime(2026, 1, 10, 8, 30, 0);
        var endDate = new DateTime(2026, 1, 20, 15, 45, 0);

        var evaluation = new Evaluation
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var expectedResult = new CountReportResponseVm();

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _pollInstanceRepositoryMock
            .Setup(x => x.GetCountReportByVariablesAsync(
                pollUuid,
                cohortIds,
                variableIds,
                true,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                evaluationId))
            .ReturnsAsync(expectedResult);

        var request = new PollCountQuery
        {
            EvaluationId = evaluationId,
            PollUuid = pollUuid,
            CohortIds = cohortIds,
            VariableIds = variableIds,
            LastVersion = true
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Same(expectedResult, result.Body);

        _evaluationRepositoryMock.Verify(
            x => x.GetByIdAsync(evaluationId),
            Times.Once);

        _pollInstanceRepositoryMock.Verify(
            x => x.GetCountReportByVariablesAsync(
                pollUuid,
                cohortIds,
                variableIds,
                true,
                It.Is<DateTime>(d =>
                    d == DateTime.SpecifyKind(startDate, DateTimeKind.Utc)),
                It.Is<DateTime>(d =>
                    d == DateTime.SpecifyKind(endDate, DateTimeKind.Utc)
                        .Date
                        .AddDays(1)
                        .AddTicks(-1)),
                evaluationId),
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

        var request = new PollCountQuery
        {
            EvaluationId = evaluationId,
            PollUuid = Guid.NewGuid().ToString(),
            CohortIds = new List<int>(),
            VariableIds = new List<int>(),
            LastVersion = false
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Failed: Evaluation not found", result.Message);
        Assert.NotNull(result.Body);

        _pollInstanceRepositoryMock.Verify(
            x => x.GetCountReportByVariablesAsync(
                It.IsAny<string>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenReportRepositoryThrowsException()
    {
        // Arrange
        var evaluationId = 123;
        var pollUuid = Guid.NewGuid().ToString();
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
            .Setup(x => x.GetCountReportByVariablesAsync(
                It.IsAny<string>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()))
            .ThrowsAsync(exception);

        var request = new PollCountQuery
        {
            EvaluationId = evaluationId,
            PollUuid = pollUuid,
            CohortIds = new List<int> { 1 },
            VariableIds = new List<int> { 10 },
            LastVersion = false
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Failed: Database error", result.Message);
        Assert.NotNull(result.Body);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenReportRepositoryReturnsNull()
    {
        // Arrange
        var evaluationId = 123;
        var pollUuid = Guid.NewGuid().ToString();

        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2026, 1, 10),
            EndDate = new DateTime(2026, 1, 20)
        };

        _evaluationRepositoryMock
            .Setup(x => x.GetByIdAsync(evaluationId))
            .ReturnsAsync(evaluation);

        _pollInstanceRepositoryMock
            .Setup(x => x.GetCountReportByVariablesAsync(
                It.IsAny<string>(),
                It.IsAny<List<int>>(),
                It.IsAny<List<int>>(),
                It.IsAny<bool>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<int>()))
            .ReturnsAsync((CountReportResponseVm?)null!);

        var request = new PollCountQuery
        {
            EvaluationId = evaluationId,
            PollUuid = pollUuid,
            CohortIds = new List<int>(),
            VariableIds = new List<int>(),
            LastVersion = true
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.StartsWith("Failed: Exception of type 'Eras.Error.Bussiness.NotFoundException' was thrown", result.Message);
        Assert.NotNull(result.Body);
    }
}