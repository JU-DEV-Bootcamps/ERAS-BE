using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class AttachmentMapper
    {
        public static Attachment ToDomain(this AttachmentEntity Entity) => new()
        {
            Id = Entity.Id,
            EntityType = Entity.EntityType,
            EntityId = Entity.EntityId,
            OriginalFileName = Entity.OriginalFileName,
            StorageKey = Entity.StorageKey,
            StorageProvider = Entity.StorageProvider,
            MimeType = Entity.MimeType,
            SizeBytes = Entity.SizeBytes,
            ContentHash = Entity.ContentHash,
            CreatedAt = Entity.CreatedAt,
            CreatedBy = Entity.CreatedBy
        };

        public static AttachmentEntity ToPersistence(this Attachment Model) => new()
        {
            Id = Model.Id,
            EntityType = Model.EntityType,
            EntityId = Model.EntityId,
            OriginalFileName = Model.OriginalFileName,
            StorageKey = Model.StorageKey,
            StorageProvider = Model.StorageProvider,
            MimeType = Model.MimeType,
            SizeBytes = Model.SizeBytes,
            ContentHash = Model.ContentHash,
            CreatedAt = Model.CreatedAt,
            CreatedBy = Model.CreatedBy
        };
    }
}
