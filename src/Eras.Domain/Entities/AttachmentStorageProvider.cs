namespace Eras.Domain.Entities;

/// <summary>
/// Identifies the storage provider backing an <see cref="Attachment"/>'s physical file.
/// Persisted as its string name (see <c>AttachmentConfiguration</c>), so the `attachments`
/// table stays human-readable and no data migration is needed when a new provider is added.
/// </summary>
public enum AttachmentStorageProvider
{
    LocalFileSystem,
}
