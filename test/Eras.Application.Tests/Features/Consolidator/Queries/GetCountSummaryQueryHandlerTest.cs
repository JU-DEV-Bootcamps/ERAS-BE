using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Consolidator.Queries;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Consolidator.Queries;
public class GetCountSummaryQueryHandlerTests
{
    private readonly Mock<ILogger<GetCountSummaryQueryHandler>> _loggerMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<ICohortRepository> _cohortRepositoryMock;
    private readonly Mock<IEvaluationRepository> _evaluationRepositoryMock;
    private readonly Mock<IPollRepository> _pollRepositoryMock;
    private readonly Mock<IPollInstanceRepository> _pollInstanceRepositoryMock;

    private readonly GetCountSummaryQueryHandler _handler;

    public GetCountSummaryQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetCountSummaryQueryHandler>>();
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _cohortRepositoryMock = new Mock<ICohortRepository>();
        _evaluationRepositoryMock = new Mock<IEvaluationRepository>();
        _pollRepositoryMock = new Mock<IPollRepository>();
        _pollInstanceRepositoryMock = new Mock<IPollInstanceRepository>();

        _handler = new GetCountSummaryQueryHandler(
            _loggerMock.Object,
            _studentRepositoryMock.Object,
            _cohortRepositoryMock.Object,
            _evaluationRepositoryMock.Object,
            _pollRepositoryMock.Object,
            _pollInstanceRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCounts_WhenAllRepositoriesSucceed()
    {
        // Arrange
        _studentRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(10);

        _cohortRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(5);

        _evaluationRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(20);

        _pollRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(8);

        _pollInstanceRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(15);

        // Act
        var result = await _handler.Handle(new GetCountSummaryQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Counts updated", result.Message);

        Assert.NotNull(result.Body);
        Assert.Equal(5, result.Body.Count);

        Assert.Equal(10, result.Body["Students"]);
        Assert.Equal(5, result.Body["Cohorts"]);
        Assert.Equal(20, result.Body["Evaluations"]);
        Assert.Equal(8, result.Body["Polls"]);
        Assert.Equal(15, result.Body["PollInstances"]);

        _studentRepositoryMock.Verify(x => x.CountAsync(), Times.Once);
        _cohortRepositoryMock.Verify(x => x.CountAsync(), Times.Once);
        _evaluationRepositoryMock.Verify(x => x.CountAsync(), Times.Once);
        _pollRepositoryMock.Verify(x => x.CountAsync(), Times.Once);
        _pollInstanceRepositoryMock.Verify(x => x.CountAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorResponse_WhenRepositoryThrowsException()
    {
        // Arrange
        var exception = new Exception("Database connection failed");

        _studentRepositoryMock
            .Setup(x => x.CountAsync())
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(new GetCountSummaryQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Error: Database connection failed", result.Message);

        Assert.NotNull(result.Body);
        Assert.Empty(result.Body);

        _studentRepositoryMock.Verify(x => x.CountAsync(), Times.Once);

        _cohortRepositoryMock.Verify(x => x.CountAsync(), Times.Never);

        _evaluationRepositoryMock.Verify(x => x.CountAsync(), Times.Never);

        _pollRepositoryMock.Verify(x => x.CountAsync(), Times.Never);

        _pollInstanceRepositoryMock.Verify(x => x.CountAsync(), Times.Never);
    }
}