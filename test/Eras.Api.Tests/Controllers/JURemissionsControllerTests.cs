using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.Remmisions.Commands.CreateRemission;
using Eras.Application.Features.Remmisions.Queries.GetRemissions;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers
{
    public class RemissionsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<RemissionsController>> _loggerMock;
        private readonly RemissionsController _controller;

        public RemissionsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<RemissionsController>>();
            _controller = new RemissionsController(_mediatorMock.Object, _loggerMock.Object);
        }

        // ---------- GetRemissionsAsync ----------

        [Fact]
        public async Task GetRemissionsAsync_ReturnsOk_WithPagedResult()
        {
            // Arrange
            var query = new Pagination { Page = 1, PageSize = 10 }; // AJUSTAR: propiedades reales de Pagination
            var expectedResult = new PagedResult<JURemission>(0, new List<JURemission>());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRemissionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var response = await _controller.GetRemissionsAsync(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task GetRemissionsAsync_PassesQueryToMediator()
        {
            // Arrange
            var query = new Pagination { Page = 2, PageSize = 5 };
            GetRemissionsQuery? capturedRequest = null;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRemissionsQuery>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<PagedResult<JURemission>>, CancellationToken>((req, _) => capturedRequest = req as GetRemissionsQuery)
                .ReturnsAsync(new PagedResult<JURemission>(0, new List<JURemission>()));

            // Act
            await _controller.GetRemissionsAsync(query);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(query, capturedRequest!.Query);
        }

        // ---------- GetRemissionByIdAsync ----------

        [Fact]
        public async Task GetRemissionByIdAsync_ReturnsOk_WithRemission()
        {
            // Arrange
            var expected = new JURemission(); // AJUSTAR: tipo real devuelto por GetRemissionByIdQuery
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRemissionByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            var response = await _controller.GetRemissionByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task GetRemissionByIdAsync_PassesIdToMediator()
        {
            // Arrange
            GetRemissionByIdQuery? capturedRequest = null;
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetRemissionByIdQuery>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<JURemission>, CancellationToken>((req, _) => capturedRequest = req as GetRemissionByIdQuery)
                .ReturnsAsync(new JURemission());

            // Act
            await _controller.GetRemissionByIdAsync(42);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(42, capturedRequest!.Id);
        }

        // ---------- CreateRemissionAsync ----------

        [Fact]
        public async Task CreateRemissionAsync_NullRemission_ReturnsBadRequest()
        {
            // Act
            var response = await _controller.CreateRemissionAsync(null!);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Remission cannot be null", badRequest.Value);

            // Verifica que se logueó el error y que nunca se llamó a MediatR
            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateRemissionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateRemissionAsync_Success_ReturnsOkWithResult()
        {
            // Arrange
            var dto = new JURemissionDTO();
            var successResult = new CreateCommandResponse<JURemission>(new JURemission(), "Success", true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateRemissionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(successResult);

            // Act
            var response = await _controller.CreateRemissionAsync(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(successResult, okResult.Value);
        }

        [Fact]
        public async Task CreateRemissionAsync_Failure_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var dto = new JURemissionDTO();
            var failureResult = new CreateCommandResponse<JURemission>(new JURemission(), "Algo salió mal", false);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateRemissionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failureResult);

            // Act
            var response = await _controller.CreateRemissionAsync(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Algo salió mal", badRequest.Value);
        }

        [Fact]
        public async Task CreateRemissionAsync_PassesRemissionToCommand()
        {
            // Arrange
            var dto = new JURemissionDTO();
            CreateRemissionCommand? capturedCommand = null;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateRemissionCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<CreateCommandResponse<JURemission>>, CancellationToken>((req, _) => capturedCommand = req as CreateRemissionCommand)
                .ReturnsAsync(new CreateCommandResponse<JURemission>(new JURemission(), "Success", true));

            // Act
            await _controller.CreateRemissionAsync(dto);

            // Assert
            Assert.NotNull(capturedCommand);
            Assert.Equal(dto, capturedCommand!.Remission);
        }
    }
}