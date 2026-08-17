namespace Eras.Application.Models;

public sealed class FileStorageSettings
{
    public required string BasePath { get; init; }
    public long MaxFileSizeBytes { get; init; } = 10_485_760; // 10 MB
    public required IReadOnlyCollection<string> AllowedExtensions { get; init; }

    /// <summary>
    /// Max attachments allowed per entity, keyed by `entityType` (e.g. "interventions"). Falls
    /// back to <see cref="DefaultMaxAttachmentsPerEntity"/> for any entity type not listed here —
    /// generalizes the previously hardcoded "5 per intervention" rule into per-entity-type,
    /// environment-configurable limits (User Story 1.4).
    /// </summary>
    public IReadOnlyDictionary<string, int> MaxAttachmentsPerEntityType { get; init; } =
        new Dictionary<string, int>();

    public int DefaultMaxAttachmentsPerEntity { get; init; } = 5;

    public int GetMaxAttachments(string entityType) =>
        MaxAttachmentsPerEntityType.TryGetValue(entityType, out int max) ? max : DefaultMaxAttachmentsPerEntity;
}
