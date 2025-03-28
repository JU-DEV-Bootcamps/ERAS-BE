using Eras.Api.Controllers;
using Eras.Application.Features.Cohort.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Eras.Api.Tests.Controllers
{
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
        public async Task GetCohorts_ReturnsOkResult()
        {
            // Arrange
            //_mediatorMock.Setup(m => m.Send(It.IsAny<GetCohortsListQuery>(), It.IsAny<CancellationToken>()))


            // Act
            var result = await _controller.GetCohorts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
    }
}
