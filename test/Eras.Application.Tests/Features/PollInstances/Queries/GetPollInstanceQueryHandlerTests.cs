using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

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
        public async Task Handle_Should_Return_Success_Response()
        {
            // Arrange
            var query = new GetPollInstanceByCohortAndDaysQuery(1, 10);
            var pollInstances = new List<PollInstance>
            {
                new PollInstance { Uuid = "uuid1", FinishedAt = DateTime.UtcNow },
                new PollInstance { Uuid = "uuid2", FinishedAt = DateTime.UtcNow.AddDays(-5) }
            };

            _mockPollInstanceRepository
                .Setup(repo => repo.GetByCohortIdAndLastDays(It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(pollInstances);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Success", result.Message);
            Assert.Equal(2, result.Body.Count());
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_Response_On_Exception()
        {
            // Arrange
            var query = new GetPollInstanceByCohortAndDaysQuery(1, 10);

            _mockPollInstanceRepository
                .Setup(repo => repo.GetByCohortIdAndLastDays(It.IsAny<int?>(), It.IsAny<int?>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed", result.Message);
            Assert.Empty(result.Body);
        }
    }
}
