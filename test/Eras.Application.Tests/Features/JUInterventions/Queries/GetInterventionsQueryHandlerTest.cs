using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Interventions.Queries.GetInterventions;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.JUInterventions.Queries;

public class GetInterventionsQueryHandlerTests
{
    private readonly Mock<IInterventionRepository> _interventionRepositoryMock;
    private readonly Mock<ILogger<GetInterventionsQueryHandler>> _loggerMock;

    private readonly GetInterventionsQueryHandler _handler;

    public GetInterventionsQueryHandlerTests()
    {
        _interventionRepositoryMock = new Mock<IInterventionRepository>();

        _loggerMock = new Mock<ILogger<GetInterventionsQueryHandler>>();

        _handler = new GetInterventionsQueryHandler(
            _interventionRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CallsCountAsyncOnce()
    {
        // Arrange
        var request = new GetInterventionsQuery
        {
            Query = new()
            {
                Page = 1,
                PageSize = 10
            }
        };

        _interventionRepositoryMock
            .Setup(x => x.GetPagedAsync(1, 10))
            .ReturnsAsync(new List<JUIntervention>());

        _interventionRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(100);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _interventionRepositoryMock.Verify(
            x => x.CountAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoInterventionsExist_ReturnsEmptyPagedResult()
    {
        // Arrange
        var request = new GetInterventionsQuery
        {
            Query = new()
            {
                Page = 1,
                PageSize = 10
            }
        };

        _interventionRepositoryMock
            .Setup(x => x.GetPagedAsync(1, 10))
            .ReturnsAsync(new List<JUIntervention>());

        _interventionRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_WhenPageHasNoResults_ButTotalCountExists_ReturnsCorrectTotalCount()
    {
        // Arrange
        var request = new GetInterventionsQuery
        {
            Query = new()
            {
                Page = 10,
                PageSize = 10
            }
        };

        _interventionRepositoryMock
            .Setup(x => x.GetPagedAsync(10, 10))
            .ReturnsAsync(new List<JUIntervention>());

        _interventionRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(95);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(95, result.Count);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_WhenCountAsyncThrows_ReturnsEmptyPagedResult()
    {
        // Arrange
        var request = new GetInterventionsQuery
        {
            Query = new()
            {
                Page = 1,
                PageSize = 10
            }
        };

        var interventions = new List<JUIntervention>
        {
            new JUIntervention
            {
                Id = 1
            }
        };

        var exception = new Exception("Database error");

        _interventionRepositoryMock
            .Setup(x => x.GetPagedAsync(1, 10))
            .ReturnsAsync(interventions);

        _interventionRepositoryMock
            .Setup(x => x.CountAsync())
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }

   
    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsSameInterventionsFromRepository()
    {
        // Arrange
        var request = new GetInterventionsQuery
        {
            Query = new()
            {
                Page = 1,
                PageSize = 5
            }
        };

        var intervention1 = new JUIntervention
        {
            Id = 101
        };

        var intervention2 = new JUIntervention
        {
            Id = 102
        };

        var interventions = new List<JUIntervention>
        {
            intervention1,
            intervention2
        };

        _interventionRepositoryMock
            .Setup(x => x.GetPagedAsync(2, 5))
            .ReturnsAsync(interventions);

        _interventionRepositoryMock
            .Setup(x => x.CountAsync())
            .ReturnsAsync(20);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Same(intervention1, result.Items[0]);
        Assert.Same(intervention2, result.Items[1]);
    }
}
