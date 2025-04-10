using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using MockQueryable.Moq;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories
{
    public class EvaluationRepositoryTest
    {
        private Mock<DbSet<EvaluationEntity>> _mockSet;
        protected Mock<AppDbContext> _mockContext;
        private IEvaluationRepository? _repository;

        public EvaluationRepositoryTest()
        {
            _mockSet = new Mock<DbSet<EvaluationEntity>>();
            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        }


        [Fact]
        public void GetByName_Should_Return()
        {
            // Arrange
            var data = new List<EvaluationEntity>
            {
                new EvaluationEntity { Id = 1, Name = "Evaluation1" },
                new EvaluationEntity { Id = 2, Name = "Evaluation2" }
            }.AsQueryable().BuildMockDbSet();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "EvaluationTest")
                .Options;

            _mockContext = new Mock<AppDbContext>(options);
            _mockContext
                .Setup(c => c.Evaluations)
                .Returns(data.Object);

            _repository = new EvaluationRepository(_mockContext.Object);

            // Act
            var result = _repository.GetByNameAsync("Evaluation1").Result;

            Assert.NotNull(result);
            Assert.Equal("Evaluation1", result.Name);
        }
    }
}
