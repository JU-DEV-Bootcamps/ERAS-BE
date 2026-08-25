using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Professionals.Queries.GetProfessionals;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Professionals.Queries;

public class GetProfessionalsQueryHandlerTest
{
    private readonly Mock<IProfessionalRepository> _repository;
    private readonly Mock<ILogger<GetProfessionalsQueryHandler>> _logger;
    private readonly GetProfessionalsQueryHandler _handler;

    public GetProfessionalsQueryHandlerTest()
    {
        _repository = new Mock<IProfessionalRepository>();
        _logger = new Mock<ILogger<GetProfessionalsQueryHandler>>();
        _handler = new GetProfessionalsQueryHandler(_repository.Object, _logger.Object);

    }

    [Fact]
    public async Task Handle_ReturnsPagedProfessionals_WhenRepositorySucceeds()
    {
        // Arrange
        var professionals = new List<JUProfessional>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        };

        var request = new GetProfessionalsQuery
        {
            Query = new()
            {
                Page = 0,
                PageSize = 10
            }
        };

        _repository
            .Setup(x => x.GetPagedAsync(1, 10))
            .ReturnsAsync(professionals);

        _repository
            .Setup(x => x.CountAsync())
            .ReturnsAsync(2);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(professionals, result.Items);

        _repository.Verify(x => x.GetPagedAsync(1, 10), Times.Once);

        _repository.Verify(x => x.CountAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyResult_WhenRepositoryThrows()
    {
        // Arrange
        var request = new GetProfessionalsQuery
        {
            Query = new()
            {
                Page = 0,
                PageSize = 10
            }
        };

        _repository
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
                    v.ToString()!.Contains("An error occurred while getting professionals")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _repository.Verify(x => x.CountAsync(), Times.Never);
    }
}
