using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Professionals.Queries.GetProfessionals;
using Eras.Application.Features.Remmisions.Queries.GetRemissions;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.JURemissions.Queries;

public class GetRemissionsQueryHandlerTest
{
    private readonly Mock<IRemissionRepository> _mockRepository;
    private readonly Mock<ILogger<GetRemissionsQuery>> _logger;
    private readonly GetRemissionsQueryHandler _handler;

    public GetRemissionsQueryHandlerTest()
    {
        _mockRepository = new Mock<IRemissionRepository>();
        _logger = new Mock<ILogger<GetRemissionsQuery>>();
        _handler = new GetRemissionsQueryHandler(_mockRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedRemissions_WhenRepositorySucceeds()
    {
        // Arrange
        var remissions = new List<JURemission>
        {
            new() { Id = 1 },
            new() { Id = 2 },
        };

        var request = new GetRemissionsQuery
        {
            Query = new()
            {
                Page = 0,
                PageSize = 10
            }
        };

        _mockRepository
            .Setup(x => x.GetPagedAsync(1, 10))
            .ReturnsAsync(remissions);

        _mockRepository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(2);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(remissions, result.Items);

        _mockRepository.Verify(x => x.GetPagedAsync(1, 10), Times.Once);
        _mockRepository.Verify(x => x.CountAsync( ), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyResult_WhenRepositoryThrows()
    {
        // Arrange
        var request = new GetRemissionsQuery
        {
            Query = new()
            {
                Page = 0,
                PageSize = 10
            }
        };

        _mockRepository
            .Setup(x => x.GetPagedAsync(1, 10))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("An error occurred while getting remissions")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockRepository.Verify(x => x.CountAsync(), Times.Never);
    }
}
