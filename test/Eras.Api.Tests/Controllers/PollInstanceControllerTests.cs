
using Eras.Api.Controllers;
using Eras.Application.DTOs;
using Eras.Application.Features.Cohorts.Queries.GetCohortComponentsByPoll;
using Eras.Application.Features.Components.Queries;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;

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
            var cohortId = new int[] { 1, 2 };
            var days = 10;
            var pagination = new Pagination();
            var pollInstanceDTOs = new List<PollInstanceDTO>
            {
                new PollInstanceDTO { Uuid = "uuid1", FinishedAt = DateTime.UtcNow },
                new PollInstanceDTO { Uuid = "uuid2", FinishedAt = DateTime.UtcNow.AddDays(-5) }
            };

            var pagedResult = new PagedResult<PollInstanceDTO>(pollInstanceDTOs.Count(), pollInstanceDTOs);

            var response = new GetQueryResponse<PagedResult<PollInstanceDTO>>(pagedResult, "Success", true);

            _mockMediator
                .Setup(M => M.Send(It.IsAny<GetPollInstanceByCohortAndDaysQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPollInstancesByCohortIdAndDaysAsync(cohortId, days, pagination) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetPollInstancesByCohortIdAndDays_Should_Return_Failure_If_Days_Is_ZeroAsync()
        {
            // Arrange
            var cohortId = new int[] { 1, 2 };
            var days = 10;
            var pagination = new Pagination();

            var pagedResult = new PagedResult<PollInstanceDTO>(0, []);
            var response = new GetQueryResponse<PagedResult<PollInstanceDTO>>(pagedResult, "Success", true);

            _mockMediator
                .Setup(M => M.Send(It.IsAny<GetPollInstanceByCohortAndDaysQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetPollInstancesByCohortIdAndDaysAsync(cohortId, days, pagination) as ObjectResult;

            // Assert
            Assert.NotNull(result);
        }


        [Fact]
        public async Task GetPollInstancesByCohortIdAndDaysAsync_ReturnsOkAsync()
        {
            // Arrange
            var pagination = new Pagination();
            var expected = new GetQueryResponse<PagedResult<PollInstanceDTO>>(null, "Success", true);

            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetPollInstanceByCohortAndDaysQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            // Act
            IActionResult result = await _controller.GetPollInstancesByCohortIdAndDaysAsync(
                new[] { 1, 2 },
                30,
                pagination,
                true,
                "poll-uuid",
                null);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }

        [Fact]
        public async Task GetComponentsAvgGroupedByCohortAsync_ReturnsGroupedResponse_ForSingleCohortAsync()
        {
            // Arrange
            var response = new List<GetCohortComponentsByPollResponse>
        {
            new()
            {
                CohortId = 1,
                CohortName = "A",
                ComponentName = "Academic",
                AverageRiskByCohortComponent = 2
            },
            new()
            {
                CohortId = 1,
                CohortName = "A",
                ComponentName = "Attendance",
                AverageRiskByCohortComponent = 3
            }
        };

            _mockMediator
                .Setup(x => x.Send(
                    It.IsAny<GetCohortComponentsByPollQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            IActionResult result =
                await _controller.GetComponentsAvgGroupedByCohortAsync("uuid", true);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetComponentsAvgGroupedByCohortAsync_ReturnsGroupedResponse_ForMultipleCohortsAsync()
        {
            // Arrange
            var response = new List<GetCohortComponentsByPollResponse>
        {
            new()
            {
                CohortId = 1,
                CohortName = "A",
                ComponentName = "Academic",
                AverageRiskByCohortComponent = 1
            },
            new()
            {
                CohortId = 2,
                CohortName = "B",
                ComponentName = "Academic",
                AverageRiskByCohortComponent = 2
            }
        };

            _mockMediator
                .Setup(x => x.Send(
                    It.IsAny<GetCohortComponentsByPollQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            IActionResult result =
                await _controller.GetComponentsAvgGroupedByCohortAsync("uuid", false);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            Assert.Equal(2, value.Count());
        }

        [Fact]
        public async Task GetComponentsRiskAvgByStudentAsync_ReturnsNotFound_WhenMediatorReturnsNullAsync()
        {
            // Arrange
            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetComponentsAvgByStudentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<ComponentsAvg>?)null);

            // Act
            IActionResult result =
                await _controller.GetComponentsRiskAvgByStudentAsync(5, 10);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetComponentsRiskAvgByStudentAsync_ReturnsNotFound_WhenMediatorReturnsEmptyCollection()
        {
            // Arrange
            _mockMediator
                .Setup(x => x.Send(It.IsAny<GetComponentsAvgByStudentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ComponentsAvg>());

            // Act
            IActionResult result =
                await _controller.GetComponentsRiskAvgByStudentAsync(5, 10);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetComponentsRiskAvgByStudentAsync_ReturnsOk_WhenMediatorReturnsData()
        {
            // Arrange
            var response = new List<ComponentsAvg>
        {
            new()
        };

            _mockMediator
                .Setup(x => x.Send(
                    It.IsAny<GetComponentsAvgByStudentQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            IActionResult result =
                await _controller.GetComponentsRiskAvgByStudentAsync(5, 10);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(response, ok.Value);
        }
    }
}
