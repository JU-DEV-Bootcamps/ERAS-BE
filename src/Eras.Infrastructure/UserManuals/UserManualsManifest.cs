using System.Text.Json;

using Eras.Domain.Entities;

namespace Eras.Infrastructure.UserManuals;

/// <summary>Reads and checks <c>manifest.json</c>, the file that declares every manual of the manuals folder.</summary>
internal static class UserManualsManifest
{
    public const string FileName = "manifest.json";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    internal sealed record Entry(string? Id, string? Title, string? Language, string? Version, string? FileName, string? Format);

    private sealed record Document(List<Entry>? Manuals);

    public static IReadOnlyList<Entry> Read(string Folder)
    {
        var path = Path.Combine(Folder, FileName);
        using FileStream stream = File.OpenRead(path);
        Document? document = JsonSerializer.Deserialize<Document>(stream, JsonOptions);
        return document?.Manuals ?? [];
    }

    public static bool TryParseFormat(string? Format, out UserManualFormat Parsed)
    {
        switch (Format?.ToLowerInvariant())
        {
            case "pdf":
                Parsed = UserManualFormat.Pdf;
                return true;
            case "odt":
                Parsed = UserManualFormat.Odt;
                return true;
            default:
                Parsed = default;
                return false;
        }
    }

    /// <summary>A plain file name: no directory part, so it cannot leave the manuals folder.</summary>
    public static bool IsPlainFileName(string? Name) =>
        !string.IsNullOrWhiteSpace(Name)
        && Name == Path.GetFileName(Name)
        && Name is not ("." or "..");
}
