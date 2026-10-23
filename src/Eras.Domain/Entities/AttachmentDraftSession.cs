using Eras.Domain.Common;

namespace Eras.Domain.Entities;

/// <summary>
/// A placeholder owning-entity row that hands out a real, persisted <c>int</c> id for
/// <see cref="Attachment.EntityId"/> to reference before the file's real owning entity (e.g. an
/// Intervention being drafted client-side) exists yet. 
/// </summary>
public class AttachmentDraftSession : BaseEntity
{
    /// <summary>The `entityType` value draft sessions use in the generic attachment endpoints.</summary>
    public const string AttachmentEntityType = "Temp";

    /// <summary>Identifier of the user who created — and thus owns — this draft session.</summary>
    public required string CreatedBy { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
