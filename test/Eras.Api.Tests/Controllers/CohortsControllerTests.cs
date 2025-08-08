using Eras.Api.Controllers;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class CohortsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<CohortsController>> _loggerMock;
    private readonly CohortsController _controller;

    public CohortsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<CohortsController>>();
        _controller = new CohortsController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCohorts_ReturnsOkResultAsync()
    {
        // Act
        IActionResult result = await _controller.GetCohortsAsync(null, false);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
