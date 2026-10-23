using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Api.Tests.Controllers;

using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Command.CreateConfiguration;
using Eras.Application.Features.Configurations.Command.DeleteConfiguration;
using Eras.Application.Features.Configurations.Command.EditConfiguration;
using Eras.Application.Features.Configurations.Queries.GetAllConfigurations;
using Eras.Application.Features.Configurations.Queries.GetUserConfigurations;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

public class ConfigurationsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ConfigurationsController>> _loggerMock;
    private readonly ConfigurationsController _controller;

    public ConfigurationsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ConfigurationsController>>();

        _controller = new ConfigurationsController(
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetConfigurationsAsync_ReturnsOkAsync()
    {
        // Arrange
        var response = new List<Configurations>();

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetAllConfigurationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.GetConfigurationsAsync();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task GetUserConfigurationsAsync_ReturnsOkAsync()
    {
        // Arrange
        var response = new List<Configurations>();

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetUserConfigurationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.GetUserConfigurationsAsync("user1");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateConfigurationAsync_ReturnsBadRequest_WhenConfigurationIsNullAsync()
    {
        // Act
        IActionResult result = await _controller.CreateConfigurationAsync(null!);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Configuration cannot be null", badRequest.Value);
    }

    [Fact]
    public async Task CreateConfigurationAsync_ReturnsOk_WhenMediatorReturnsSuccessAsync()
    {
        // Arrange
        var dto = new ConfigurationsDTO
        {
            ConfigurationName = "Test",
            BaseURL = "fake/url",
            UserId = "test-user",
            EncryptedKey = "fake-key"
        };
        var response = new CreateCommandResponse<Configurations>(
            new Configurations
            {
                UserId = "test-user",
                ConfigurationName = "Test",
                BaseURL = "fake/url",
                EncryptedKey = "fake-key"
            },
            "Success", true); 

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateConfigurationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateConfigurationAsync(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateConfigurationAsync_ReturnsBadRequest_WhenMediatorReturnsFailureAsync()
    {
        // Arrange
        var dto = new ConfigurationsDTO
        {
            ConfigurationName = "Test",
            BaseURL = "fake/url",
            UserId = "test-user",
            EncryptedKey = "fake-key"
        };
        var response = new CreateCommandResponse<Configurations>(
            new Configurations
            {
                UserId = "test-user",
                ConfigurationName = "Test",
                BaseURL = "fake/url",
                EncryptedKey = "fake-key"
            },
            "Error", false);

        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateConfigurationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateConfigurationAsync(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Message, badRequest.Value);
    }

    [Fact]
    public async Task EditConfigurationAsync_ReturnsBadRequest_WhenConfigurationIsNullAsync()
    {
        // Act
        IActionResult result = await _controller.EditConfigurationAsync(null!);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Configuration cannot be null", badRequest.Value);
    }

    [Fact]
    public async Task EditConfigurationAsync_ReturnsOk_WhenMediatorReturnsSuccessAsync()
    {
        // Arrange
        var dto = new ConfigurationsDTO
        {
            ConfigurationName = "Test",
            BaseURL = "fake/url",
            UserId = "test-user",
            EncryptedKey = "fake-key"
        };
        var response = new CreateCommandResponse<Configurations>(
            new Configurations
            {
                UserId = "test-user",
                ConfigurationName = "Test",
                BaseURL = "fake/url",
                EncryptedKey = "fake-key"
            },
            "Success", true);

        _mediatorMock
            .Setup(X => X.Send(
                It.IsAny<EditConfigurationCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.EditConfigurationAsync(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task EditConfigurationAsync_ReturnsBadRequest_WhenMediatorReturnsFailureAsync()
    {
        // Arrange
        var dto = new ConfigurationsDTO
        {
            ConfigurationName = "Test",
            BaseURL = "fake/url",
            UserId = "test-user",
            EncryptedKey = "fake-key"
        };
        var response = new CreateCommandResponse<Configurations>(
            new Configurations
            {
                UserId = "test-user",
                ConfigurationName = "Test",
                BaseURL = "fake/url",
                EncryptedKey = "fake-key"
            },
            "Error", false);

        _mediatorMock
            .Setup(X => X.Send(
                It.IsAny<EditConfigurationCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.EditConfigurationAsync(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Message, badRequest.Value);
    }

    [Fact]
    public async Task DeleteConfigurationAsync_ReturnsBadRequest_WhenIdIsInvalidAsync()
    {
        // Act
        IActionResult result = await _controller.DeleteConfigurationAsync(0);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ConfigurationId must be greater than 0", badRequest.Value);
    }

    [Fact]
    public async Task DeleteConfigurationAsync_ReturnsOk_WhenMediatorReturnsSuccessAsync()
    {
        // Arrange
        var response = new BaseResponse
        {
            Success = true,
            Message = "Deleted"
        };
        _mediatorMock
            .Setup(X => X.Send(
                It.IsAny<DeleteConfigurationCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.DeleteConfigurationAsync(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task DeleteConfigurationAsync_ReturnsBadRequest_WhenMediatorReturnsFailureAsync()
    {
        // Arrange
        var response = new BaseResponse
        {
            Success = false,
            Message = "Delete failed"
        };

        _mediatorMock
            .Setup(X => X.Send(
                It.IsAny<DeleteConfigurationCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.DeleteConfigurationAsync(1);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Message, badRequest.Value);
    }
}
