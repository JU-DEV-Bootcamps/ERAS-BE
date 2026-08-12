using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.JUServices.Commands.CreateJUService;
using Eras.Application.Features.JUServices.Queries.GetJUServices;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class JUServiceControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<JUServiceController>> _loggerMock;
    private readonly JUServiceController _controller;

    public JUServiceControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<JUServiceController>>();

        _controller = new JUServiceController(
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetJUServicesAsync_ReturnsOkAsync()
    {
        // Arrange
        var pagination = new Pagination();
        var response = new PagedResult<JUService>(Items:
            [new JUService()
            {
                Id = 1,
                Name = "Test",
            }],
            Count: 1
        );
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetJUServicesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.GetServicesAsync(pagination);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateJUServiceAsync_ReturnsBadRequest_WhenJUServiceIsNullAsync()
    {
        // Act
        IActionResult result = await _controller.CreateServiceAsync(null!);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Service cannot be null", badRequest.Value);
    }

    [Fact]
    public async Task CreateJUServiceAsync_ReturnsOk_WhenMediatorReturnsSuccessAsync()
    {
        // Arrange
        var dto = new JUServiceDTO() { Name = "b" };
        var service = new JUService();
        var response = new CreateCommandResponse<JUService>(service, "Success", true);
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateJUServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateServiceAsync(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateJUServiceAsync_ReturnsBadRequest_WhenMediatorReturnsFailureAsync()
    {
        // Arrange
        var dto = new JUServiceDTO() { Name = "b" };
        var response = new CreateCommandResponse<JUService>(null!, "Error", false);
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateJUServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateServiceAsync(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Message, badRequest.Value);
    }
}
