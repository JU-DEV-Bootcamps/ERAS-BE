using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Consolidator.Queries.Polls;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Consolidator.Queries;

public class GetRiskCountQueryHandlerTests
{
    private readonly Mock<IPollVariableRepository> _pollVariableRepositoryMock;
    private readonly Mock<ILogger<GetRiskCountQueryHandler>> _loggerMock;
    private readonly GetRiskCountQueryHandler _handler;

    public GetRiskCountQueryHandlerTests()
    {
        _pollVariableRepositoryMock = new Mock<IPollVariableRepository>();
        _loggerMock = new Mock<ILogger<GetRiskCountQueryHandler>>();

        _handler = new GetRiskCountQueryHandler(
            _pollVariableRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnRiskCounts_WhenAnswersExist()
    {
        // Arrange
        var pollUuid = Guid.NewGuid();

        var answers = new List<Answer>
        {
            new() { RiskLevel = 0 },
            new() { RiskLevel = 1 },
            new() { RiskLevel = 1 },
            new() { RiskLevel = 2.5m },
            new() { RiskLevel = 3.8m }
        };

        _pollVariableRepositoryMock
            .Setup(x => x.GetAnswersByPollUuidAsync(pollUuid.ToString()))
            .ReturnsAsync(answers);

        var request = new GetRiskCountQuery
        {
            PollUuid = pollUuid
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Body);

        Assert.Equal(5, result.Body.AnswerCount);
        Assert.Equal(1.66m, result.Body.AverageRisk);

        var risk0 = result.Body.Risks.Single(x => x.StartRange == 0);
        Assert.Equal("Risk 0 - 1", risk0.Label);
        Assert.Equal(0, risk0.StartRange);
        Assert.Equal(1, risk0.EndRange);
        Assert.Equal(1, risk0.Count);

        var risk1 = result.Body.Risks.Single(x => x.StartRange == 1);
        Assert.Equal("Risk 1 - 2", risk1.Label);
        Assert.Equal(1, risk1.StartRange);
        Assert.Equal(2, risk1.EndRange);
        Assert.Equal(2, risk1.Count);

        var risk2 = result.Body.Risks.Single(x => x.StartRange == 2);
        Assert.Equal("Risk 2 - 3", risk2.Label);
        Assert.Equal(2, risk2.StartRange);
        Assert.Equal(3, risk2.EndRange);
        Assert.Equal(1, risk2.Count);

        var risk3 = result.Body.Risks.Single(x => x.StartRange == 3);
        Assert.Equal("Risk 3 - 4", risk3.Label);
        Assert.Equal(3, risk3.StartRange);
        Assert.Equal(4, risk3.EndRange);
        Assert.Equal(1, risk3.Count);

        _pollVariableRepositoryMock.Verify(
            x => x.GetAnswersByPollUuidAsync(pollUuid.ToString()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldGroupRiskLevelsFiveAndAboveIntoRiskFourPlus()
    {
        // Arrange
        var pollUuid = Guid.NewGuid();

        var answers = new List<Answer>
        {
            new() { RiskLevel = 4.5m },
            new() { RiskLevel = 5 },
            new() { RiskLevel = 6 },
            new() { RiskLevel = 10 }
        };

        _pollVariableRepositoryMock
            .Setup(x => x.GetAnswersByPollUuidAsync(pollUuid.ToString()))
            .ReturnsAsync(answers);

        var request = new GetRiskCountQuery
        {
            PollUuid = pollUuid
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Body);

        var risk4Plus = Assert.Single(result.Body.Risks);

        Assert.Equal("Risk 4+", risk4Plus.Label);
        Assert.Equal(4, risk4Plus.StartRange);
        Assert.Equal(5, risk4Plus.EndRange);
        Assert.Equal(4, risk4Plus.Count);

        Assert.Equal(6.38m, result.Body.AverageRisk);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        // Arrange
        var pollUuid = Guid.NewGuid();
        var exception = new Exception("Database error");

        _pollVariableRepositoryMock
            .Setup(x => x.GetAnswersByPollUuidAsync(pollUuid.ToString()))
            .ThrowsAsync(exception);

        var request = new GetRiskCountQuery
        {
            PollUuid = pollUuid
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(
            "Failed to retrieve risk count by poll. Error Database error", result.Message);

        Assert.NotNull(result.Body);
        Assert.Equal(0, result.Body.AverageRisk);

        _pollVariableRepositoryMock.Verify(x => x.GetAnswersByPollUuidAsync(pollUuid.ToString()),
            Times.Once);
    }
}
