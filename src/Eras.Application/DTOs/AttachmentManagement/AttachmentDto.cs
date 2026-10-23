namespace Eras.Application.DTOs.AttachmentManagement;

public sealed record AttachmentDto
{
    public int Id { get; init; }
    public required string EntityType { get; init; }
    public required int EntityId { get; init; }
    public string? OriginalFileName { get; init; }
    public string? MimeType { get; init; }
    public long? SizeBytes { get; init; }
    public required string ContentHash { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Direct-access download URL, when the active storage provider supports one (e.g. a future
    /// Swift Temporary URL). Null for the local provider — clients should fall back to
    /// <c>GET /attachments/{id}/download</c>, which streams the file through the backend instead.
    /// </summary>
    public string? DownloadUrl { get; init; }
}
