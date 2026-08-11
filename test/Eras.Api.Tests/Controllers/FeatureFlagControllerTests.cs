namespace Eras.Api.Tests.Controllers;

using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.FeatureFlags;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

public class FeatureFlagControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<FeatureFlagController>> _loggerMock;
    private readonly FeatureFlagController _controller;

    public FeatureFlagControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<FeatureFlagController>>();

        _controller = new FeatureFlagController(
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetFeatureFlagByNameAsync_ReturnsOk_WhenFeatureFlagExistsAsync()
    {
        // Arrange
        var dto = new FeatureFlagDTO
        {
            Id = 1,
            Name = "FeatureA",
            Description = "Description",
            IsEnabled = true,
            Audit = null!,
        };

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetFeatureFlagByNameQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _controller.GetFeatureFlagByNameAsync("FeatureA");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(dto, ok.Value);
    }

    [Fact]
    public async Task GetFeatureFlagByNameAsync_ReturnsNotFound_WhenExceptionThrownAsync()
    {
        // Arrange
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetFeatureFlagByNameQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("error"));

        // Act
        var result = await _controller.GetFeatureFlagByNameAsync("FeatureA");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkAsync()
    {
        // Arrange
        IReadOnlyCollection<FeatureFlagDTO> response =
        [
            new FeatureFlagDTO
            {
                Id = 1,
                Name = "FeatureA",
                Description = "Description",
                IsEnabled = true,
                Audit = null!,
            }
        ];

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetAllFeatureFlagsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateFeatureFlagAsync_ReturnsCreated_WhenSuccessfulAsync()
    {
        // Arrange
        var dto = new FeatureFlagDTO
        {
            Id = 1,
            Name = "FeatureA",
            Description = "Description",
            IsEnabled = true,
            Audit = null!,
        };

        var created = new FeatureFlagDTO
        {
            Id = 1,
            Name = "FeatureA",
            Description = "Description",
            IsEnabled = true,
            Audit = null!,
        };

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateFeatureFlagCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _controller.CreateFeatureFlagAsync(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Same(created, createdResult.Value);
    }

    [Fact]
    public async Task CreateFeatureFlagAsync_ReturnsConflict_WhenAlreadyExistsAsync()
    {
        // Arrange
        var dto = new FeatureFlagDTO
        {
            Id = 1,
            Name = "FeatureA",
            Description = "Description",
            IsEnabled = true,
            Audit = null!,
        };

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateFeatureFlagCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Already exists"));

        // Act
        var result = await _controller.CreateFeatureFlagAsync(dto);

        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdatedFeatureFlagAsync_ReturnsOk_AndUsesRouteIdAsync()
    {
        // Arrange
        var dto = new FeatureFlagDTO
        {
            Id = 99,
            Name = "FeatureA",
            Description = "Description",
            IsEnabled = true,
            Audit = null!,
        };

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<UpdateFeatureFlagCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeatureFlagDTO
            {
                Id = 5,
                Name = "FeatureA",
                Description = "Description",
                IsEnabled = true,
                Audit = null!,
            });

        // Act
        var result = await _controller.UpdatedFeatureFlagAsync(5, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<FeatureFlagDTO>(ok.Value);

        Assert.Equal(5, response.Id);
    }

    [Fact]
    public async Task DeleteFeatureFlagAsync_ReturnsNoContent_WhenSuccessfulAsync()
    {
        // Arrange
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteFeatureFlagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteFeatureFlagAsync(10);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteFeatureFlagAsync_ReturnsNotFound_WhenKeyDoesNotExistAsync()
    {
        // Arrange
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<DeleteFeatureFlagCommand>(),It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _controller.DeleteFeatureFlagAsync(10);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}