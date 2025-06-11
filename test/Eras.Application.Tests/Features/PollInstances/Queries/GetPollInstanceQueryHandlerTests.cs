using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Eras.Application.Utils;
using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Queries
{
    public class GetPollInstanceQueryHandlerTests
    {
        private readonly Mock<IPollInstanceRepository> _mockPollInstanceRepository;
        private readonly Mock<ILogger<GetPollInstanceByCohortAndDaysQueryHandler>> _mockLogger;
        private readonly GetPollInstanceByCohortAndDaysQueryHandler _handler;

        public GetPollInstanceQueryHandlerTests()
        {
            _mockPollInstanceRepository = new Mock<IPollInstanceRepository>();
            _mockLogger = new Mock<ILogger<GetPollInstanceByCohortAndDaysQueryHandler>>();
            _handler = new GetPollInstanceByCohortAndDaysQueryHandler(_mockPollInstanceRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Success_ResponseAsync()
        {
            var cohortId = new int[] { 1, 2 };
            var days = 10;
            var pagination = new Pagination();


            // Arrange
            var query = new GetPollInstanceByCohortAndDaysQuery(pagination, cohortId, days);
            var pollInstances = new List<PollInstance>
            {
                new PollInstance { Uuid = "uuid1", FinishedAt = DateTime.UtcNow },
                new PollInstance { Uuid = "uuid2", FinishedAt = DateTime.UtcNow.AddDays(-5) }
            };
            var pagedResult = new PagedResult<PollInstance>(pollInstances.Count(), pollInstances);

            _mockPollInstanceRepository
                .Setup(Repo => Repo.GetByCohortIdAndLastDays(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<int?>()))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Success", result.Message);
            Assert.Equal(2, result.Body.Count);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_Response_On_ExceptionAsync()
        {
            var cohortId = new int[] { 1, 2 };
            var days = 10;
            var pagination = new Pagination();


            // Arrange
            var query = new GetPollInstanceByCohortAndDaysQuery(pagination, cohortId, days);

            _mockPollInstanceRepository
                .Setup(Repo => Repo.GetByCohortIdAndLastDays(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed", result.Message);
            Assert.Empty(result.Body.Items);
        }
    }
}
