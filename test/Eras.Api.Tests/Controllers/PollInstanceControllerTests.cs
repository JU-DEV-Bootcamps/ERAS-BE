
using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Api.Tests.Controllers
{
    public class PollInstanceControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ILogger<StudentsController>> _mockLogger;
        private readonly PollInstancesController _controller;

        public PollInstanceControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<StudentsController>>();
            _controller = new PollInstancesController(_mockMediator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetPollInstancesByCohortIdAndDays_Should_Return_Success_ResponseAsync()
        {
            // Arrange
            var cohortId = 1;
            var days = 10;
            var pollInstanceDTOs = new List<PollInstanceDTO>
            {
                new PollInstanceDTO { Uuid = "uuid1", FinishedAt = DateTime.UtcNow },
                new PollInstanceDTO { Uuid = "uuid2", FinishedAt = DateTime.UtcNow.AddDays(-5) }
            };
            var response = new GetQueryResponse<IEnumerable<PollInstanceDTO>>(pollInstanceDTOs, "Success", true);

            _mockMediator
                .Setup(M => M.Send(It.IsAny<GetPollInstanceByCohortAndDaysQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPollInstancesByCohortIdAndDaysAsync(cohortId, days) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetPollInstancesByCohortIdAndDays_Should_Return_Failure_ResponseAsync()
        {
            // Arrange
            var cohortId = 1;
            var days = 10;
            var response = new GetQueryResponse<IEnumerable<PollInstanceDTO>>(new List<PollInstanceDTO>(), "Failed", false);

            _mockMediator
                .Setup(M => M.Send(It.IsAny<GetPollInstanceByCohortAndDaysQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPollInstancesByCohortIdAndDaysAsync(cohortId, days) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }
    }
}
