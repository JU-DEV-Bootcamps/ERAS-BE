using Eras.Domain.Common;

namespace Eras.Domain.Entities;

/// <summary>
/// Generic, entity-agnostic file attachment metadata record. A single row describes one
/// physical file (identified by <see cref="StorageKey"/> within <see cref="StorageProvider"/>)
/// attached to an arbitrary owning entity (<see cref="EntityType"/> + <see cref="EntityId"/>).
/// </summary>
public class Attachment : BaseEntity
{
    /// <summary>Type of the owning entity (e.g. "Intervention"). Validated against a whitelist by the service layer.</summary>
    public required string EntityType { get; init; }

    /// <summary>Id of the specific owning entity instance.</summary>
    public required int EntityId { get; init; }

    /// <summary>
    /// Original file name as uploaded by the user. Null for rows backfilled from legacy data,
    /// where this information was never captured historically.
    /// </summary>
    public string? OriginalFileName { get; init; }

    /// <summary>
    /// Location of the file within the storage provider. For the Local File Storage provider this
    /// is the path on the file system; for OpenStack Swift it is the object name within its container.
    /// </summary>
    public required string StorageKey { get; init; }

    /// <summary>Identifier of the storage provider that holds the file. Initially always <see cref="AttachmentStorageProvider.LocalFileSystem"/>.</summary>
    public AttachmentStorageProvider StorageProvider { get; init; } = AttachmentStorageProvider.LocalFileSystem;

    /// <summary>
    /// File MIME type. Null for rows backfilled from legacy data, where this information was
    /// never captured historically.
    /// </summary>
    public string? MimeType { get; init; }

    /// <summary>
    /// File size in bytes. Null for rows backfilled from legacy data, where this information was
    /// never captured historically.
    /// </summary>
    public long? SizeBytes { get; init; }

    /// <summary>SHA-256 hash of the file content, in the same format already produced by the current upload handler.</summary>
    public required string ContentHash { get; init; }

    /// <summary>Upload date of the file (UTC).</summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Identifier of the user who uploaded the file.</summary>
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Set when this row's <see cref="EntityType"/>/<see cref="EntityId"/> were just reassigned
    /// but the physical file at <see cref="StorageKey"/> still lives under its old
    /// location — relocation is a separate, out-of-band step. Set to Null once the file has been moved
    /// (or for a row that was never reassigned).
    /// </summary>
    public DateTime? StorageRelocationPendingAt { get; init; }
}
