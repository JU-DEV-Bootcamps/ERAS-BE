using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Queries.GetAllConfigurations;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Configurations.Queries;

public class GetAllConfigurationsQueryHandlerTest
{
    private readonly Mock<IConfigurationsRepository> _configurationsRepository;
    private readonly Mock<ILogger<GetAllConfigurationsQueryHandler>> _logger;
    private readonly GetAllConfigurationsQueryHandler _handler;

    public GetAllConfigurationsQueryHandlerTest()
    {
        _configurationsRepository = new Mock<IConfigurationsRepository>();
        _logger = new Mock<ILogger<GetAllConfigurationsQueryHandler>>();

        _handler = new GetAllConfigurationsQueryHandler(
            _configurationsRepository.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllConfigurations()
    {
        // Arrange
        var configurations = new List<Domain.Entities.Configurations>
        {
            new Domain.Entities.Configurations
            {
                Id = 1,
                ConfigurationName = "Configuration 1",
                EncryptedKey = "plain-key",
                UserId = "1",
                BaseURL = "12345/asfhj",
            },
            new Domain.Entities.Configurations
            {
                Id = 1,
                ConfigurationName = "Configuration 2",
                EncryptedKey = "plain-key",
                UserId = "1",
                BaseURL = "12345/asfhj",
            }
        };

        var request = new GetAllConfigurationsQuery();

        _configurationsRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(configurations);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Equal(configurations[0].Id, result[0].Id);

        Assert.Equal(configurations[1].Id, result[1].Id);

        _configurationsRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }
}
