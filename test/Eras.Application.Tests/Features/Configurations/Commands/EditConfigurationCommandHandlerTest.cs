using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Command.EditConfiguration;
using Eras.Domain.Common;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Configurations.Commands;

public class EditConfigurationCommandHandlerTests
{
    private readonly Mock<IConfigurationsRepository> _configurationsRepository;
    private readonly Mock<IServiceProvidersRepository> _serviceProvidersRepository;
    private readonly Mock<ILogger<EditConfigurationCommandHandler>> _logger;
    private readonly Mock<IApiKeyEncryptor> _encryptor;
    private readonly EditConfigurationCommandHandler _handler;

    public EditConfigurationCommandHandlerTests()
    {
        _configurationsRepository = new Mock<IConfigurationsRepository>();
        _serviceProvidersRepository = new Mock<IServiceProvidersRepository>();
        _logger = new Mock<ILogger<EditConfigurationCommandHandler>>();
        _encryptor = new Mock<IApiKeyEncryptor>();
        _handler = new EditConfigurationCommandHandler(
            _configurationsRepository.Object,
            _serviceProvidersRepository.Object,
            _logger.Object, _encryptor.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenConfigurationDoesNotExist()
    {
        // Arrange
        var request = new EditConfigurationCommand
        {
            ConfigurationDTO = new ConfigurationsDTO
            {
                Id = 1,
                ConfigurationName = "Test Configuration",
                EncryptedKey = "plain-key",
                UserId = "1",
                BaseURL = "12345/asfhj",
            }
        };

        _configurationsRepository
            .Setup(x => x.GetByIdAsyncNoTracking(1))
            .ReturnsAsync((Domain.Entities.Configurations?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Entity);
        Assert.Equal("Configuration not found", result.Message);
        Assert.False(result.Success);

        _configurationsRepository.Verify(
            x => x.GetByIdAsyncNoTracking(1),
            Times.Once);

        _serviceProvidersRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _configurationsRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Configurations>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenServiceProviderDoesNotExist()
    {
        // Arrange
        var configuration = new Domain.Entities.Configurations
        {
            Id = 1,
            ConfigurationName = "Old Configuration",
            EncryptedKey = "plain-key",
            UserId = "1",
            BaseURL = "12345/asfhj",
            Audit = new AuditInfo()
            {
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = "one"
            }
        };

        var request = new EditConfigurationCommand
        {
            ConfigurationDTO = new ConfigurationsDTO
            {
                Id = 1,
                ConfigurationName = "Updated Configuration",
                ServiceProviderId = 1,
                BaseURL = "https://example.com",
                EncryptedKey = "my-key",
                UserId = "test-user"
            }
        };

        _configurationsRepository
            .Setup(x => x.GetByIdAsyncNoTracking(1))
            .ReturnsAsync(configuration);

        _encryptor
            .Setup(x => x.Encrypt("my-key"))
            .Returns("encrypted-key");

        _serviceProvidersRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.ServiceProviders?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ServiceProvider not found", result.Message);
        Assert.False(result.Success);

        _encryptor.Verify(
            x => x.Encrypt("my-key"),
            Times.Once);

        _serviceProvidersRepository.Verify(
            x => x.GetByIdAsync(1),
            Times.Once);

        _configurationsRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Domain.Entities.Configurations>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateConfiguration_WhenConfigurationAndServiceProviderExist()
    {
        // Arrange
        var configuration = new Domain.Entities.Configurations
        {
            Id = 1,
            ConfigurationName = "Old Configuration",
            ServiceProviderId = 1,
            UserId = "2",
            BaseURL = "https://old.example.com",
            EncryptedKey = "old-key",
            Audit = new AuditInfo()
            {
                ModifiedBy = "me",
                ModifiedAt = DateTime.UtcNow,
            }
        };

        var serviceProvider = new Domain.Entities.ServiceProviders
        {
            Id = 1,
            ServiceProviderLogo = "logo",
            ServiceProviderName = "name",
        };

        var request = new EditConfigurationCommand
        {
            ConfigurationDTO = new ConfigurationsDTO
            {
                Id = 1,
                ConfigurationName = "Updated Configuration",
                ServiceProviderId = 1,
                BaseURL = "https://new.example.com",
                EncryptedKey = "plain-key",
                UserId = "test-user"
            }
        };

        _configurationsRepository
            .Setup(x => x.GetByIdAsyncNoTracking(1))
            .ReturnsAsync(configuration);

        _encryptor
            .Setup(x => x.Encrypt("plain-key"))
            .Returns("encrypted-key");

        _serviceProvidersRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(serviceProvider);

        //_configurationsRepository
        //    .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Configurations>()))
        //    .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Same(configuration, result.Entity);
        //Assert.Equal(1, result.Count);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);

        Assert.Equal("Updated Configuration", configuration.ConfigurationName);
        Assert.Equal(1, configuration.ServiceProviderId);
        Assert.Equal("https://new.example.com", configuration.BaseURL);
        Assert.Equal("encrypted-key", configuration.EncryptedKey);
        Assert.Equal("test-user", configuration.Audit.ModifiedBy);
        Assert.Same(serviceProvider, configuration.ServiceProvider);

        _encryptor.Verify(x => x.Encrypt("plain-key"), Times.Once);

        _serviceProvidersRepository.Verify(x => x.GetByIdAsync(1), Times.Once);

        _configurationsRepository.Verify(x => x.UpdateAsync(configuration), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new EditConfigurationCommand
        {
            ConfigurationDTO = new ConfigurationsDTO
            {
                Id = 1,
                ConfigurationName = "Test Configuration",
                EncryptedKey = "plain-key",
                UserId = "1",
                BaseURL = "12345/asfhj",
            }
        };

        _configurationsRepository
            .Setup(x => x.GetByIdAsyncNoTracking(1))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Entity);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);

        _configurationsRepository.Verify(x => x.GetByIdAsyncNoTracking(1), Times.Once);

        _configurationsRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Configurations>()), Times.Never);
    }
}
