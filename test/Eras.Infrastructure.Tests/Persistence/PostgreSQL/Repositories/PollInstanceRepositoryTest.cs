using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories
{   

    public class PollInstanceRepositoryTest
    {
        private Mock<DbSet<PollInstanceEntity>> _mockSet;
        protected Mock<AppDbContext> _mockContext;
        private PollInstanceRepository? _repository;

        public PollInstanceRepositoryTest()
        {
            _mockSet = new Mock<DbSet<PollInstanceEntity>>();
            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        }


        [Fact]
        public void GetByLastDays_Should_Return()
        {
            // Arrange
            var data = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, FinishedAt = DateTime.UtcNow, StudentId = 1 },
                new PollInstanceEntity { Id = 2, FinishedAt = DateTime.UtcNow.AddDays(-100), StudentId = 1 }
            }.AsQueryable().BuildMockDbSet();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UserServiceTest")
                .Options;

            _mockContext = new Mock<AppDbContext>(options);
            _mockContext
                .Setup(C => C.PollInstances)
                .Returns(data.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            // Act
            var result = _repository.GetByLastDays(10).Result;

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDaysShouldReturnByDaysAsync()
        {
            // Arrange
            var data = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, FinishedAt = DateTime.UtcNow, StudentId = 1 },
                new PollInstanceEntity { Id = 2, FinishedAt = DateTime.UtcNow.AddDays(-100), StudentId = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockContext.Setup(C => C.PollInstances).Returns(data.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            // Act
            var result = await _repository.GetByCohortIdAndLastDays(null, 10);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDaysShouldReturnByCohortIdAndDaysAsync()
        {
            // Arrange
            var data = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, FinishedAt = DateTime.UtcNow, StudentId = 1 },
                new PollInstanceEntity { Id = 2, FinishedAt = DateTime.UtcNow.AddDays(-100), StudentId = 2 }
            }.AsQueryable().BuildMockDbSet();

            var studentCohorts = new List<StudentCohortJoin>
            {
                new StudentCohortJoin { StudentId = 1, CohortId = 1 },
                new StudentCohortJoin { StudentId = 2, CohortId = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockContext.Setup(C => C.PollInstances).Returns(data.Object);
            _mockContext.Setup(C => C.StudentCohorts).Returns(studentCohorts.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            // Act
            var result = await _repository.GetByCohortIdAndLastDays(1, 10);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDaysShouldReturnAllWhenBothParametersAreZeroAsync()
        {
            // Arrange
            var data = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, FinishedAt = DateTime.UtcNow, StudentId = 1 },
                new PollInstanceEntity { Id = 2, FinishedAt = DateTime.UtcNow.AddDays(-100), StudentId = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockContext.Setup(C => C.PollInstances).Returns(data.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            // Act
            var result = await _repository.GetByCohortIdAndLastDays(0, 0);

            // Assert
            Assert.Equal(2, result.Count());
        }

    }
}
