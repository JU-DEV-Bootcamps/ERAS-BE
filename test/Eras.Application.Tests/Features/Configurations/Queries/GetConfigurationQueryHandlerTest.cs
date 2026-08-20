
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Queries.GetConfiguration;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Configurations.Queries;

public class GetConfigurationQueryHandlerTest
{
    private readonly Mock<IConfigurationsRepository> _configurationRepository;
    private readonly Mock<ILogger<GetConfigurationQueryHandler>> _logger;
    private readonly GetConfigurationQueryHandler _handler;

    public GetConfigurationQueryHandlerTest()
    {
        _configurationRepository = new Mock<IConfigurationsRepository>();
        _logger = new Mock<ILogger<GetConfigurationQueryHandler>>();
        _handler = new GetConfigurationQueryHandler(
            _configurationRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldGetConfiguration_Successfully()
    {
        // Arrange 
        var configuration = new Domain.Entities.Configurations
        {
            Id = 1,
            ConfigurationName = "Test Configuration",
            UserId = "1",
            BaseURL = "1235/jkl",
            EncryptedKey = "1235/jkl",
        };
        var request = new GetConfigurationQuery
        {
            ConfigurationId = 1
        };

        _configurationRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(configuration);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Configuration", result.ConfigurationName);

        _configurationRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenConfigurationIsNull()
    {
        var request = new GetConfigurationQuery
        {
            ConfigurationId = 1
        };

        _configurationRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Configurations)null!);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _handler.Handle(request, CancellationToken: CancellationToken.None));
    }
}
