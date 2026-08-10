using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.Professionals.Commands.CreateProfessional;
using Eras.Application.Features.Professionals.Queries.GetProfessionals;
using Eras.Application.Models.Response;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;
namespace Eras.Api.Tests.Controllers;

public class ProfessionalsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ProfessionalsController>> _loggerMock;
    private readonly ProfessionalsController _controller;

    public ProfessionalsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ProfessionalsController>>();

        _controller = new ProfessionalsController(
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetProfessionalsAsync_ReturnsOkAsync()
    {
        // Arrange
        var pagination = new Pagination();
        var response = new PagedResult<JUProfessional>( Items:
            [new JUProfessional()
            {
                Id = 1,
                Name = "Test",
                Uuid = "36",
            }],
            Count: 1
        );
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<GetProfessionalsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.GetProfessionalsAsync(pagination);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateProfessionalAsync_ReturnsBadRequest_WhenProfessionalIsNullAsync()
    {
        // Act
        IActionResult result = await _controller.CreateProfessionalAsync(null!);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Professional cannot be null", badRequest.Value);
    }

    [Fact]
    public async Task CreateProfessionalAsync_ReturnsOk_WhenMediatorReturnsSuccessAsync()
    {
        // Arrange
        var dto = new JUProfessionalDTO() { Name = "b" };
        var professional = new JUProfessional();
        var response = new CreateCommandResponse<JUProfessional>(professional, "Success", true);
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateProfessionalCommand>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateProfessionalAsync(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task CreateProfessionalAsync_ReturnsBadRequest_WhenMediatorReturnsFailureAsync()
    {
        // Arrange
        var dto = new JUProfessionalDTO() { Name = "b" };
        var response = new CreateCommandResponse<JUProfessional>(null!, "Error", false);
        _mediatorMock
            .Setup(X => X.Send(It.IsAny<CreateProfessionalCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        IActionResult result = await _controller.CreateProfessionalAsync(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(response.Message, badRequest.Value);
    }
}