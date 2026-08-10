using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.ServiceProviders.Command;
using Eras.Application.Features.ServiceProviders.Queries;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class ServiceProvidersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ServiceProvidersController>> _loggerMock;
    private readonly ServiceProvidersController _controller;

    public ServiceProvidersControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ServiceProvidersController>>();
        _controller = new ServiceProvidersController(
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetServiceProvidersAsync_ReturnsOkAsync()
    {
        // Arrange
        var response = new List<ServiceProviders>([
            new ServiceProviders()
            {
                ServiceProviderName = "Name",
                ServiceProviderLogo = "Test",
            }
        ]);
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetAllServiceProvidersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.GetServiceProvidersAsync();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateServiceProviderAsync_ReturnsBadRequest_WhenServiceProviderIsNullAsync()
    {
        // Act
        IActionResult result = await _controller.CreateServiceProviderAsync(null!);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Service Provider cannot be null", badRequest.Value);
    }

    [Fact]
    public async Task CreateServiceProviderAsync_ReturnsOk_WhenMediatorReturnsSuccessAsync()
    {
        // Arrange
        var dto = new ServiceProvidersDTO() { ServiceProviderName = "", ServiceProviderLogo = "b" };
        var serviceProvider = new ServiceProviders() 
        {
            ServiceProviderName = "A",
            ServiceProviderLogo = "logo",
        };
        var response = new CreateCommandResponse<ServiceProviders>(serviceProvider, "Success", true);
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateServiceProviderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateServiceProviderAsync(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateServiceProviderAsync_ReturnsBadRequest_WhenMediatorReturnsFailureAsync()
    {
        // Arrange
        var dto = new ServiceProvidersDTO() { ServiceProviderName = "", ServiceProviderLogo = "b" };
        var response = new CreateCommandResponse<ServiceProviders>(null!, "Error", false);
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateServiceProviderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateServiceProviderAsync(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Message, badRequest.Value);
    }
}
