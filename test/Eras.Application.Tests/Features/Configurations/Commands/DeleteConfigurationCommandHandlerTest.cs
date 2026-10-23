using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Configurations.Command.DeleteConfiguration;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Configurations.Commands;

public class DeleteConfigurationCommandHandlerTest
{
    private readonly Mock<IConfigurationsRepository> _configurationsRepository;
    private readonly Mock<ILogger<DeleteConfigurationCommandHandler>> _logger;
    private readonly DeleteConfigurationCommandHandler _handler;

    public DeleteConfigurationCommandHandlerTest()
    {
        _configurationsRepository = new Mock<IConfigurationsRepository>();
        _logger = new Mock<ILogger<DeleteConfigurationCommandHandler>>();

        _handler = new DeleteConfigurationCommandHandler(
            _configurationsRepository.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteConfiguration_WhenConfigurationExists()
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

        var request = new DeleteConfigurationCommand
        {
            ConfigurationId = 1
        };

        _configurationsRepository
            .Setup(x => x.GetByIdAsyncNoTracking(1))
            .ReturnsAsync(configuration);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Configuration deleted", result.Message);
        Assert.True(result.Success);

        _configurationsRepository.Verify(
            x => x.GetByIdAsyncNoTracking(1),
            Times.Once);

        _configurationsRepository.Verify(
            x => x.UpdateDeleteStatus(1),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenConfigurationDoesNotExist()
    {
        // Arrange
        var request = new DeleteConfigurationCommand
        {
            ConfigurationId = 1
        };

        _configurationsRepository
            .Setup(x => x.GetByIdAsyncNoTracking(1))
            .ReturnsAsync((Domain.Entities.Configurations)null!);

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Configuration not found", result.Message);
        Assert.False(result.Success);

        _configurationsRepository.Verify(
            x => x.GetByIdAsyncNoTracking(1),
            Times.Once);

        _configurationsRepository.Verify(
            x => x.UpdateDeleteStatus(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRepositoryThrowsException()
    {
        // Arrange
        var request = new DeleteConfigurationCommand
        {
            ConfigurationId = 1
        };

        _configurationsRepository
            .Setup(x => x.GetByIdAsyncNoTracking(1))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Error", result.Message);
        Assert.False(result.Success);

        _configurationsRepository.Verify(
            x => x.GetByIdAsyncNoTracking(1),
            Times.Once);

        _configurationsRepository.Verify(
            x => x.UpdateDeleteStatus(It.IsAny<int>()),
            Times.Never);
    }
}