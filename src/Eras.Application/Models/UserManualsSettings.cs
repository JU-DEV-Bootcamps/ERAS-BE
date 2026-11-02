namespace Eras.Application.Models;

public sealed class UserManualsSettings
{
    /// <summary>Folder that holds <c>manifest.json</c> and the manual files.</summary>
    public string Path { get; init; } = "/app/manuals";

    public long MaxFileSizeBytes { get; init; } = 10_485_760; // 10 MB
}
