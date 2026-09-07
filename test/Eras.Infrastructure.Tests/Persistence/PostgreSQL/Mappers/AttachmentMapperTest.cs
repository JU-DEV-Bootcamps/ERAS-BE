using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class AttachmentMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_AttachmentEntity_To_Attachment()
        {
            var entity = new AttachmentEntity
            {
                Id = 1,
                EntityType = "Intervention",
                EntityId = 42,
                OriginalFileName = "report.pdf",
                StorageKey = "Intervention/42/file.pdf",
                StorageProvider = AttachmentStorageProvider.LocalFileSystem,
                MimeType = "application/pdf",
                SizeBytes = 12345,
                ContentHash = new string('a', 64),
                CreatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "user-uuid-1"
            };

            var result = entity.ToDomain();

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.EntityType, result.EntityType);
            Assert.Equal(entity.EntityId, result.EntityId);
            Assert.Equal(entity.OriginalFileName, result.OriginalFileName);
            Assert.Equal(entity.StorageKey, result.StorageKey);
            Assert.Equal(entity.StorageProvider, result.StorageProvider);
            Assert.Equal(entity.MimeType, result.MimeType);
            Assert.Equal(entity.SizeBytes, result.SizeBytes);
            Assert.Equal(entity.ContentHash, result.ContentHash);
            Assert.Equal(entity.CreatedAt, result.CreatedAt);
            Assert.Equal(entity.CreatedBy, result.CreatedBy);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Attachment_To_AttachmentEntity()
        {
            var model = new Attachment
            {
                Id = 1,
                EntityType = "Intervention",
                EntityId = 42,
                OriginalFileName = "report.pdf",
                StorageKey = "Intervention/42/file.pdf",
                StorageProvider = AttachmentStorageProvider.LocalFileSystem,
                MimeType = "application/pdf",
                SizeBytes = 12345,
                ContentHash = new string('a', 64),
                CreatedAt = new DateTime(2026, 8, 11, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "user-uuid-1"
            };

            var result = model.ToPersistence();

            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.EntityType, result.EntityType);
            Assert.Equal(model.EntityId, result.EntityId);
            Assert.Equal(model.OriginalFileName, result.OriginalFileName);
            Assert.Equal(model.StorageKey, result.StorageKey);
            Assert.Equal(model.StorageProvider, result.StorageProvider);
            Assert.Equal(model.MimeType, result.MimeType);
            Assert.Equal(model.SizeBytes, result.SizeBytes);
            Assert.Equal(model.ContentHash, result.ContentHash);
            Assert.Equal(model.CreatedAt, result.CreatedAt);
            Assert.Equal(model.CreatedBy, result.CreatedBy);
        }

        [Fact]
        public void ToDomain_Should_MapNullLegacyOnlyFields_AsNull()
        {
            var entity = new AttachmentEntity
            {
                Id = 1,
                EntityType = "Intervention",
                EntityId = 7,
                StorageKey = "Intervention/7/legacy-file.bin",
                ContentHash = new string('c', 64),
                CreatedBy = "migration-script"
            };

            var result = entity.ToDomain();

            Assert.Null(result.OriginalFileName);
            Assert.Null(result.MimeType);
            Assert.Null(result.SizeBytes);
        }

        [Fact]
        public void ToDomain_And_ToPersistence_Should_RoundTrip_StorageRelocationPendingAt()
        {
            var relocationPendingAt = new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc);
            var entity = new AttachmentEntity
            {
                Id = 1,
                EntityType = "Intervention",
                EntityId = 42,
                StorageKey = "Intervention/42/file.pdf",
                ContentHash = new string('a', 64),
                CreatedBy = "user-uuid-1",
                StorageRelocationPendingAt = relocationPendingAt
            };

            Attachment domain = entity.ToDomain();
            Assert.Equal(relocationPendingAt, domain.StorageRelocationPendingAt);

            AttachmentEntity roundTripped = domain.ToPersistence();
            Assert.Equal(relocationPendingAt, roundTripped.StorageRelocationPendingAt);
        }
    }
}
