using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Models.Response.HeatMap;
using Eras.Error.Bussiness;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Heatmap.Queries.GetHeatmapSummary;

public class GetHeatMapSummaryHandlerTests
{
    private readonly Mock<IHeatMapRepository> _heatMapRepositoryMock;
    private readonly Mock<ILogger<GetHeatMapSummaryHandler>> _loggerMock;
    private readonly GetHeatMapSummaryHandler _handler;

    public GetHeatMapSummaryHandlerTests()
    {
        _heatMapRepositoryMock = new Mock<IHeatMapRepository>();
        _loggerMock = new Mock<ILogger<GetHeatMapSummaryHandler>>();

        _handler = new GetHeatMapSummaryHandler(
            _heatMapRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenPollInstanceUuidIsNull_ThrowsNotFoundException()
    {
        // Arrange
        var request = new GetHeatMapSummaryQuery(null!);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.StartsWith("Exception of type 'Eras.Error.Bussiness.", exception.Message);

        _heatMapRepositoryMock.Verify(
            x => x.GetHeatMapAnswersPercentageByVariableAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPollInstanceUuidIsWhitespace_ReturnsSuccess()
    {
        // Arrange
        var pollInstanceUuid = "   ";

        var request = new GetHeatMapSummaryQuery(pollInstanceUuid);

        var repositoryResponse =
            new List<GetHeatMapAnswersPercentageByVariableQueryResponse>();

        _heatMapRepositoryMock
            .Setup(x => x.GetHeatMapAnswersPercentageByVariableAsync(pollInstanceUuid))
            .ReturnsAsync(repositoryResponse);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsData_ReturnsSuccessResponse()
    {
        // Arrange
        var pollInstanceUuid = "poll-instance-123";
        var request = new GetHeatMapSummaryQuery(pollInstanceUuid);

        var repositoryResponse =
            new List<GetHeatMapAnswersPercentageByVariableQueryResponse>
            {
                new GetHeatMapAnswersPercentageByVariableQueryResponse()
            };

        _heatMapRepositoryMock
            .Setup(x => x.GetHeatMapAnswersPercentageByVariableAsync(
                pollInstanceUuid))
            .ReturnsAsync(repositoryResponse);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Body);

        _heatMapRepositoryMock.Verify(
            x => x.GetHeatMapAnswersPercentageByVariableAsync(pollInstanceUuid),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmptyCollection_ReturnsSuccessResponse()
    {
        // Arrange
        var pollInstanceUuid = "poll-instance-123";
        var request = new GetHeatMapSummaryQuery(pollInstanceUuid);

        _heatMapRepositoryMock
            .Setup(x => x.GetHeatMapAnswersPercentageByVariableAsync(pollInstanceUuid))
            .ReturnsAsync(new List<GetHeatMapAnswersPercentageByVariableQueryResponse>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Body);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailedResponse()
    {
        // Arrange
        var pollInstanceUuid = "poll-instance-123";
        var repositoryException = new Exception("Database error");

        var request = new GetHeatMapSummaryQuery(pollInstanceUuid);

        _heatMapRepositoryMock
            .Setup(x => x.GetHeatMapAnswersPercentageByVariableAsync(pollInstanceUuid))
            .ThrowsAsync(repositoryException);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Failed", result.Message);
        Assert.NotNull(result.Body);
        Assert.IsType<HeatMapSummaryResponseVm>(result.Body);

        _heatMapRepositoryMock.Verify(
            x => x.GetHeatMapAnswersPercentageByVariableAsync(pollInstanceUuid),
            Times.Once);
    }
}
