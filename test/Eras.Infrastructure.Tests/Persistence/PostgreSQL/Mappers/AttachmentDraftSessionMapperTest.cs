using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class AttachmentDraftSessionMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_AttachmentDraftSessionEntity_To_AttachmentDraftSession()
        {
            var entity = new AttachmentDraftSessionEntity
            {
                Id = 1,
                CreatedBy = "user-1",
                CreatedAt = new DateTime(2026, 8, 26, 0, 0, 0, DateTimeKind.Utc),
            };

            var result = entity.ToDomain();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.CreatedBy, result.CreatedBy);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public void ToPersistence_Should_Convert_AttachmentDraftSession_To_AttachmentDraftSessionEntity()
        {
            var model = new AttachmentDraftSession
            {
                Id = 1,
                CreatedBy = "user-1",
                CreatedAt = new DateTime(2026, 8, 26, 0, 0, 0, DateTimeKind.Utc),
            };

            var result = model.ToPersistence();

            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.CreatedBy, result.CreatedBy);
            Assert.Equal(model.CreatedAt, result.CreatedAt);
        }
    }
}
