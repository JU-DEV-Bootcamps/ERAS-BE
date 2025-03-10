using Eras.Api.Controllers;
using Eras.Application.Features.Cohort.Queries;
using Eras.Application.Models;
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
        public async Task GetCohortsSummary_EmptyResponse()
        {
            var fakeResponse = new GetQueryResponse<List<(Student Student, List<PollInstance> PollInstances)>>([]);
            // Arrange
            // _mediatorMock
            //     .Setup(m => m.Send(It.IsAny<GetCohortsSummaryQuery>(), It.IsAny<CancellationToken>()))
            //     .ReturnsAsync(fakeResponse);
        }
    }

    // Mock classes for the test
    public class GetCohortsSummaryQueryItem
    {
        public Student Student { get; set; } = new Student();
        public List<PollInstance> PollInstances { get; set; } = [new PollInstance()];
    }

    public class Student
    {
        public string Uuid { get; set; } = new Guid().ToString();
        public string Name { get; set; } = "Random student";
        public Cohort Cohort { get; set; } = new Cohort();
    }

    public class Cohort
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; } = "Class 1999";
    }

    public class PollInstance
    {
        public List<Answer> Answers { get; set; } = [new Answer()];
    }

    public class Answer
    {
        public double RiskLevel { get; set; } = 3.5;
    }
}
