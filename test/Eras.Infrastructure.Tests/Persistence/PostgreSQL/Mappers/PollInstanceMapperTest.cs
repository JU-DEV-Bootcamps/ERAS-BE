using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class PollInstanceMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_PollInstanceEntity_To_PollInstance()
        {
            var entity = new PollInstanceEntity
            {
                Id = 1,
                Uuid = "1234",
                Student = new StudentEntity { Id = 1, Name = "Test Student" },
                FinishedAt = DateTime.Now
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Uuid, result.Uuid);
            Assert.NotNull(result.Student);
            Assert.Equal(entity.Student.Id, result.Student.Id);
            Assert.Equal(entity.Audit, result.Audit);
            Assert.Equal(entity.FinishedAt, result.FinishedAt);
        }


        [Fact]
        public void ToPersistence_Should_Convert_PollInstance_To_PollInstanceEntity()
        {
            // Arrange
            var model = new PollInstance
            {
                Id = 1,
                Uuid = "1234",
                Student = new Student { Id = 1, Name = "Test Student" },
                FinishedAt = DateTime.Now
            };

            // Act
            var result = model.ToPersistence();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Uuid, result.Uuid);
            Assert.Equal(model.Student.Id, result.StudentId);
            Assert.Equal(model.Audit, result.Audit);
            Assert.Equal(model.FinishedAt, result.FinishedAt);
        }
    }
}
