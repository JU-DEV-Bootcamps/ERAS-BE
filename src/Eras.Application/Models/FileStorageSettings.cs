namespace Eras.Application.Models;

public sealed class FileStorageSettings
{
    public required string BasePath { get; init; }
    public long MaxFileSizeBytes { get; init; } = 10_485_760; // 10 MB
    public required IReadOnlyCollection<string> AllowedExtensions { get; init; }
}