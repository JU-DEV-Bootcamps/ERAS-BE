using Eras.Api.Controllers;
using Eras.Application.Features.Cohort.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Api.Tests.Controllers
{
    public class CohortsControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<CohortsController>> _loggerMock;
        private readonly CohortsController _controller;

        public CohortsControllerTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<CohortsController>>();
            _controller = new CohortsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetCohortsSummary_ReturnsBadRequest_OnException()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCohortsSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception());

            // Act
            var result = await _controller.GetCohortsSummary();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }

    // Mock classes for the test
    public class GetCohortsSummaryQueryResponse
    {
        public Student Student { get; set; }
        public List<PollInstance> PollInstances { get; set; }
    }

    public class Student
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public Cohort Cohort { get; set; }
    }

    public class Cohort
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PollInstance
    {
        public List<Answer> Answers { get; set; }
    }

    public class Answer
    {
        public int RiskLevel { get; set; }
    }
}
