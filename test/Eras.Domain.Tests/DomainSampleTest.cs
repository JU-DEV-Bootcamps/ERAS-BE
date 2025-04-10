using Eras.Domain.Common;
using Eras.Domain.Entities;

using Moq;

namespace Eras.Domain.Tests
{
    public class DomainSampleTest
    {
        [Fact]
        public async Task GetAllPollsAsync_ShouldReturnListOfPolls()
        {
            // Arrange
            var mockRepo = new Mock<IPollRepositorySample>();
            //-DB Data example
            var expectedPolls = new List<Poll>
            {
                new Poll {
                    Id = 1,
                    Name = "Poll 1",
                    Audit = new AuditInfo { CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now }
                },
                new Poll {
                    Id = 2,
                    Name = "Poll 2",
                    Audit = new AuditInfo { CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now }
                }
            };

            mockRepo.Setup(repo => repo.GetAllPollsAsync()).ReturnsAsync(expectedPolls);

            // Act
            var actualPolls = await mockRepo.Object.GetAllPollsAsync();

            // Assert
            Assert.Equal(expectedPolls, actualPolls);
        }
    }
}
