using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class PollMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_PollEntity_To_Poll()
        {
            // Arrange
            var entity = new PollEntity
            {
                Id = 1,
                Name = "Test Poll",
                Version = "1",
                Uuid = "1234",
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Version, result.Version);
            Assert.Equal(entity.Uuid, result.Uuid);
            Assert.Equal(entity.Audit, result.Audit);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Poll_To_PollEntity()
        {
            var model = new Poll
            {
                Id = 1,
                Name = "Test Poll",
                Version = "1",
                Uuid = "1234",
            };
            var result = model.ToPersistence();
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.Version, result.Version);
            Assert.Equal(model.Uuid, result.Uuid);
            Assert.Equal(model.Audit, result.Audit);
        }
    }
}
