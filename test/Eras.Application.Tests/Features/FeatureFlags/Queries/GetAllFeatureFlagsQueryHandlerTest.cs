using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.FeatureFlags;
using Eras.Application.Features.FeatureFlags.Handlers.QueryHandlers;
using Eras.Domain.Entities.FeatureFlagManagement;

using Moq;

namespace Eras.Application.Tests.Features.FeatureFlags.Queries;

public class GetAllFeatureFlagsQueryHandlerTests
{
    private readonly Mock<IFeatureFlagRepository> _repositoryMock;
    private readonly GetAllFeatureFlagsQueryHandler _handler;

    public GetAllFeatureFlagsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IFeatureFlagRepository>();
        _handler = new GetAllFeatureFlagsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllFeatureFlagsAsDTOs()
    {
        // Arrange
        var entities = new List<FeatureFlag>
        {
            new FeatureFlag
            {
                Name = "v2"
            },
            new FeatureFlag
            {
                Name = "v1"
            }
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(entities);

        // Act
        var result = await _handler.Handle(new GetAllFeatureFlagsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entities.Count, result.Count);

        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmpty_ShouldReturnEmptyCollection()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<FeatureFlag>());

        // Act
        var result = await _handler.Handle(new GetAllFeatureFlagsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
}
