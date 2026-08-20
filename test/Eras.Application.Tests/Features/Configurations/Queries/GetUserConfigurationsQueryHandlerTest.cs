using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Queries.GetUserConfigurations;
using Eras.Application.Models.Response.HeatMap;
using Eras.Domain.Common;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Configurations.Queries;

public class GetUserConfigurationsQueryHandlerTests
{
    private readonly Mock<IConfigurationsRepository> _repositoryMock;
    private readonly Mock<ILogger<GetUserConfigurationsQueryHandler>> _loggerMock;
    private readonly Mock<IApiKeyEncryptor> _encryptorMock;
    private readonly GetUserConfigurationsQueryHandler _handler;

    public GetUserConfigurationsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IConfigurationsRepository>();
        _loggerMock = new Mock<ILogger<GetUserConfigurationsQueryHandler>>();
        _encryptorMock = new Mock<IApiKeyEncryptor>();
        _handler = new GetUserConfigurationsQueryHandler(
            _repositoryMock.Object,
            _loggerMock.Object,
            _encryptorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnConfigurations_FromRepository()
    {
        // Arrange
        var userId = "user-123";

        var configurations = new List<Domain.Entities.Configurations>
        {
            new()
            {
                EncryptedKey = "null",
                UserId = userId,
                BaseURL = "baserUrl",
                ConfigurationName = "newConfig",
            },
            new()
            {
                EncryptedKey = "null2",
                UserId = userId,
                BaseURL = "baserUrl",
                ConfigurationName = "newConfig",
            }
        };

        _repositoryMock
            .Setup(x => x.GetUserConfigurationsAsync(userId))
            .ReturnsAsync(configurations);

        var request = new GetUserConfigurationsQuery
        {
            UserId = userId
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Same(configurations[0], result[0]);
        Assert.Same(configurations[1], result[1]);

        _repositoryMock.Verify(x => x.GetUserConfigurationsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldDecryptEncryptedKey()
    {
        // Arrange
        var userId = "user-123";
        var encryptedKey = "encrypted-key";
        var decryptedKey = "decrypted-key";

        var configuration = new Domain.Entities.Configurations
        {
            EncryptedKey = encryptedKey,
            UserId = userId,
            BaseURL = "baserUrl",
            ConfigurationName = "newConfig",
        };

        _repositoryMock
            .Setup(x => x.GetUserConfigurationsAsync(userId))
            .ReturnsAsync(new List<Domain.Entities.Configurations>
            {
                configuration
            });

        _encryptorMock
            .Setup(x => x.Decrypt(encryptedKey))
            .Returns(decryptedKey);

        var request = new GetUserConfigurationsQuery
        {
            UserId = userId
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(decryptedKey, result[0].EncryptedKey);

        _encryptorMock.Verify(x => x.Decrypt(encryptedKey), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task Handle_ShouldNotDecrypt_WhenEncryptedKeyIsNullOrWhitespace(string encryptedKey)
    {
        // Arrange
        var configuration = new Domain.Entities.Configurations
        {
            EncryptedKey = encryptedKey,
            UserId = "123",
            BaseURL = "baserUrl",
            ConfigurationName = "newConfig",
        };

        _repositoryMock
            .Setup(x => x.GetUserConfigurationsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<Domain.Entities.Configurations>
            {
                configuration
            });

        var request = new GetUserConfigurationsQuery
        {
            UserId = "user-123"
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(encryptedKey, result[0].EncryptedKey);

        _encryptorMock.Verify(x => x.Decrypt(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDecryptOnlyConfigurationsWithEncryptedKeys()
    {
        // Arrange
        var userId = "user-123";

        var configurationWithKey = new Domain.Entities.Configurations
        {
            EncryptedKey = "encrypted-key",
            UserId = userId,
            BaseURL = "baserUrl",
            ConfigurationName = "newConfig",
        };

        var configurationWithoutKey = new Domain.Entities.Configurations
        {
            EncryptedKey = "null",
            UserId = userId,
            BaseURL = "baserUrl",
            ConfigurationName = "newConfig",
        };

        var configurationWithWhitespace = new Domain.Entities.Configurations
        {
            EncryptedKey = " ",
            UserId = userId,
            BaseURL = "baserUrl",
            ConfigurationName = "newConfig",
        };

        _repositoryMock
            .Setup(x => x.GetUserConfigurationsAsync(userId))
            .ReturnsAsync(new List<Domain.Entities.Configurations>
            {
                configurationWithKey,
                configurationWithoutKey,
                configurationWithWhitespace
            });

        _encryptorMock
            .Setup(x => x.Decrypt("encrypted-key"))
            .Returns("decrypted-key");

        var request = new GetUserConfigurationsQuery
        {
            UserId = userId
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("decrypted-key", result[0].EncryptedKey);
        Assert.Null(result[1].EncryptedKey);
        Assert.Equal(" ", result[2].EncryptedKey);

        _encryptorMock.Verify(x => x.Decrypt("encrypted-key"), Times.Once);

        _encryptorMock.Verify(x => x.Decrypt(It.Is<string>(x => string.IsNullOrWhiteSpace(x))), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDecryptAllEncryptedKeys()
    {
        // Arrange
        var userId = "user-123";

        var configurations = new List<Domain.Entities.Configurations>
        {
            new() {EncryptedKey = "encrypted-1", UserId = userId, BaseURL = "baserUrl", ConfigurationName = "newConfig"},
            new() {EncryptedKey = "encrypted-2", UserId = userId, BaseURL = "baserUrl", ConfigurationName = "newConfig"},
            new() {EncryptedKey = "encrypted-3", UserId = userId, BaseURL = "baserUrl", ConfigurationName = "newConfig"}
        };

        _repositoryMock
            .Setup(x => x.GetUserConfigurationsAsync(userId))
            .ReturnsAsync(configurations);

        _encryptorMock
            .Setup(x => x.Decrypt("encrypted-1"))
            .Returns("decrypted-1");

        _encryptorMock
            .Setup(x => x.Decrypt("encrypted-2"))
            .Returns("decrypted-2");

        _encryptorMock
            .Setup(x => x.Decrypt("encrypted-3"))
            .Returns("decrypted-3");

        var request = new GetUserConfigurationsQuery
        {
            UserId = userId
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);

        Assert.Equal("decrypted-1", result[0].EncryptedKey);
        Assert.Equal("decrypted-2", result[1].EncryptedKey);
        Assert.Equal("decrypted-3", result[2].EncryptedKey);

        _encryptorMock.Verify(
            x => x.Decrypt("encrypted-1"),
            Times.Once);

        _encryptorMock.Verify(
            x => x.Decrypt("encrypted-2"),
            Times.Once);

        _encryptorMock.Verify(
            x => x.Decrypt("encrypted-3"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassUserIdToRepository()
    {
        // Arrange
        var userId = "specific-user-id";

        _repositoryMock
            .Setup(x => x.GetUserConfigurationsAsync(userId))
            .ReturnsAsync(new List<Domain.Entities.Configurations>());

        var request = new GetUserConfigurationsQuery
        {
            UserId = userId
        };

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetUserConfigurationsAsync(userId), Times.Once);
    }
}
