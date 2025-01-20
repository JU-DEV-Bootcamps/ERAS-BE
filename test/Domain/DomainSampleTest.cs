using ERAS.Infrastructure.Persistence.PostgreSQL;
using Moq;

namespace ERAS.Domain.Tests
{
    public class DomainSampleTest
    {
        [Fact]
        public async Task GetAllPollsAsync_ShouldReturnListOfPolls()
        {
            // Arrange
            var mockRepo = new Mock<IPollRepositorySample>();
            //-DB Data example
            var expectedPolls = new List<Polls>
            {
                new Polls { Id = 1, Name = "Poll 1", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow },
                new Polls { Id = 2, Name = "Poll 2", CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
            };

            mockRepo.Setup(repo => repo.GetAllPollsAsync()).ReturnsAsync(expectedPolls);

            // Act
            var actualPolls = await mockRepo.Object.GetAllPollsAsync();

            // Assert
            Assert.Equal(expectedPolls, actualPolls);
        }
    }
}
