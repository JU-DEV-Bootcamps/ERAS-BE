
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.HeatMap;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByCohort;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Heatmap.Queries.GetHeatMapDetailsByCohort
{
    public class GetHeatMapDetailsByCohortQueryHandlerTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<ILogger<GetHeatMapDetailsByCohortQueryHandler>> _mockLogger;
        private readonly GetHeatMapDetailsByCohortQueryHandler _handler;

        public GetHeatMapDetailsByCohortQueryHandlerTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockLogger = new Mock<ILogger<GetHeatMapDetailsByCohortQueryHandler>>();
            _handler = new GetHeatMapDetailsByCohortQueryHandler(
                _mockStudentRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ReturnsListOfStudentHeatMapDetailDto()
        {
            var cohortId = "1";
            var limit = 5;
            var query = new GetHeatMapDetailsByCohortQuery(cohortId, limit);

            var expectedList = new List<StudentHeatMapDetailDto>
            {
                new StudentHeatMapDetailDto { StudentId = 1, StudentName = "Student 1", RiskLevel = 90, ComponentName = "Cohort 1" },
                new StudentHeatMapDetailDto { StudentId = 2, StudentName = "Student 2", RiskLevel = 85, ComponentName = "Cohort 1" }
            };

            _mockStudentRepository
                .Setup(repo => repo.GetStudentHeatMapDetailsByCohort(cohortId, limit))
                .ReturnsAsync(expectedList);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(expectedList.Count, result.Count);
            _mockStudentRepository.Verify(repo => repo.GetStudentHeatMapDetailsByCohort(cohortId, limit), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoDataFound()
        {
            var cohortId = "2";
            var limit = 5;
            var query = new GetHeatMapDetailsByCohortQuery(cohortId, limit);

            var expectedList = new List<StudentHeatMapDetailDto>();

            _mockStudentRepository
                .Setup(repo => repo.GetStudentHeatMapDetailsByCohort(cohortId, limit))
                .ReturnsAsync(expectedList);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockStudentRepository.Verify(repo => repo.GetStudentHeatMapDetailsByCohort(cohortId, limit), Times.Once);
        }
    }
}

