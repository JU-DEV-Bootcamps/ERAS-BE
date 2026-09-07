using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class AttachmentEntity : BaseEntity
    {
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string? OriginalFileName { get; set; }
        public string StorageKey { get; set; } = string.Empty;
        public AttachmentStorageProvider StorageProvider { get; set; } = AttachmentStorageProvider.LocalFileSystem;
        public string? MimeType { get; set; }
        public long? SizeBytes { get; set; }
        public string ContentHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? StorageRelocationPendingAt { get; set; }
    }
}
