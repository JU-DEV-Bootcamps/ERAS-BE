using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Command.CreateConfiguration;
using Eras.Domain.Common;
using Eras.Error.Bussiness;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Configurations.Commands;

public class CreateConfigurationCommandHandlerTest
{
    private readonly Mock<IConfigurationsRepository> _configurationsRepository;
    private readonly Mock<ILogger<CreateConfigurationCommandHandler>> _logger;
    private readonly Mock<IApiKeyEncryptor> _encryptor;

    private readonly CreateConfigurationCommandHandler _handler;

    public CreateConfigurationCommandHandlerTest()
    {
        _configurationsRepository = new Mock<IConfigurationsRepository>();
        _logger = new Mock<ILogger<CreateConfigurationCommandHandler>>();
        _encryptor = new Mock<IApiKeyEncryptor>();

        _handler = new CreateConfigurationCommandHandler(
            _configurationsRepository.Object,
            _logger.Object,
            _encryptor.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenConfigurationIsNull()
    {
        // Arrange
        var request = new CreateConfigurationCommand
        {
            Configurations = null
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);

        _configurationsRepository.Verify(
            x => x.GetActiveByUserAndNameAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldRejectWithConflict_WhenTheUserAlreadyHasAnActiveConfigurationWithThatName()
    {
        var request = new CreateConfigurationCommand
        {
            Configurations = new ConfigurationsDTO
            {
                ConfigurationName = "Existing Configuration",
                UserId = "1",
                BaseURL = "https://new.example.com",
                EncryptedKey = "new-plain-key",
            }
        };

        _configurationsRepository
            .Setup(x => x.GetActiveByUserAndNameAsync("1", "Existing Configuration"))
            .ReturnsAsync(new Domain.Entities.Configurations
            {
                Id = 5,
                ConfigurationName = "Existing Configuration",
                UserId = "1",
                BaseURL = "https://old.example.com",
                EncryptedKey = "anything"
            });

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(
            () => _handler.Handle(request, CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.Contains("Existing Configuration", error.FriendlyMessage);
        _configurationsRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Configurations>()), Times.Never);
        _encryptor.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldLookTheNameUpOnlyAmongTheConfigurationsOfThatUser()
    {
        var request = new CreateConfigurationCommand
        {
            Configurations = new ConfigurationsDTO
            {
                ConfigurationName = "Shared Name",
                UserId = "user-b",
                BaseURL = "12345/asfhj",
                EncryptedKey = "plain-key",
            }
        };
        _configurationsRepository
            .Setup(x => x.GetActiveByUserAndNameAsync("user-b", "Shared Name"))
            .ReturnsAsync((Domain.Entities.Configurations?)null);
        _encryptor.Setup(x => x.Encrypt("plain-key")).Returns("encrypted-key");
        _configurationsRepository
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Configurations>()))
            .ReturnsAsync((Domain.Entities.Configurations C) => C);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.Success);
        _configurationsRepository.Verify(x => x.GetActiveByUserAndNameAsync("user-b", "Shared Name"), Times.Once);
        _configurationsRepository.Verify(x => x.GetActiveByUserAndNameAsync("user-a", It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateConfiguration_WhenConfigurationDoesNotExist()
    {
        // Arrange
        var request = new CreateConfigurationCommand
        {
            Configurations = new ConfigurationsDTO
            {
                ConfigurationName = "New Configuration",
                EncryptedKey = "plain-key",
                UserId = "1",
                BaseURL = "12345/asfhj",
            }
        };

        var createdConfiguration = new Domain.Entities.Configurations
        {
            ConfigurationName = "New Configuration",
            EncryptedKey = "encrypted-key",
            UserId = "1",
            BaseURL = "12345/asfhj"
        };

        _configurationsRepository
            .Setup(x => x.GetActiveByUserAndNameAsync("1", "New Configuration"))
            .ReturnsAsync((Domain.Entities.Configurations?)null);

        _encryptor
            .Setup(x => x.Encrypt("plain-key"))
            .Returns("encrypted-key");

        _configurationsRepository
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Configurations>()))
            .ReturnsAsync(createdConfiguration);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Same(createdConfiguration, result.Entity);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);

        _encryptor.Verify(
            x => x.Encrypt("plain-key"),
            Times.Once);

        _configurationsRepository.Verify(
            x => x.AddAsync(
                It.Is<Domain.Entities.Configurations>(
                    c => c.EncryptedKey == "encrypted-key")),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateConfigurationWithoutEncrypting_WhenEncryptedKeyIsEmpty()
    {
        // Arrange
        var request = new CreateConfigurationCommand
        {
            Configurations = new ConfigurationsDTO
            {
                ConfigurationName = "New Configuration",
                EncryptedKey = "",
                UserId = "1",
                BaseURL = "12345/asfhj"
            }
        };

        var createdConfiguration = new Domain.Entities.Configurations
        {
            ConfigurationName = "New Configuration",
            EncryptedKey = "",
            UserId = "1",
            BaseURL = "12345/asfhj"
        };

        _configurationsRepository
            .Setup(x => x.GetActiveByUserAndNameAsync("1", "New Configuration"))
            .ReturnsAsync((Domain.Entities.Configurations?)null);

        _configurationsRepository
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Configurations>()))
            .ReturnsAsync(createdConfiguration);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Success", result.Message);
        Assert.True(result.Success);

        _encryptor.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Never);

        _configurationsRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Configurations>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new CreateConfigurationCommand
        {
            Configurations = new ConfigurationsDTO
            {
                ConfigurationName = "New Configuration",
                EncryptedKey = "plain-key",
                UserId = "1",
                BaseURL = "12345/asfhj"
            }
        };

        _configurationsRepository
            .Setup(x => x.GetActiveByUserAndNameAsync("1", "New Configuration"))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);
    }
}
