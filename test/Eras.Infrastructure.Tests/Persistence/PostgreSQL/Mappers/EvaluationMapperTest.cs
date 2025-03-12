using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class EvaluationMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_EvaluationEntity_To_Evaluation()
        {
            // Arrange
            var entity = new EvaluationEntity
            {
                Id = 1,
                Name = "Test Evaluation",
                Status = "Active",
                PollName = "Test Poll",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };

            // Act
            var result = entity.ToDomain();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Status, result.Status);
            Assert.Equal(entity.PollName, result.PollName);
            Assert.Equal(entity.StartDate, result.StartDate);
            Assert.Equal(entity.EndDate, result.EndDate);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Evaluation_To_EvaluationEntity()
        {
            // Arrange
            var model = new Evaluation
            {
                Id = 1,
                Name = "Test Evaluation",
                Status = "Active",
                PollName = "Test Poll",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };

            // Act
            var result = EvaluationMapper.ToPersistence(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.Status, result.Status);
            Assert.Equal(model.PollName, result.PollName);
            Assert.Equal(model.StartDate, result.StartDate);
            Assert.Equal(model.EndDate, result.EndDate);
        }
    }
}
