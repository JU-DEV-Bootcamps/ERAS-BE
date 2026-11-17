using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Models;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Microsoft.Extensions.Options;

namespace Eras.Infrastructure.UserManuals;

/// <summary>Reads the manuals declared in <c>manifest.json</c> from the configured manuals folder.</summary>
public sealed class FileSystemUserManualCatalog(IOptions<UserManualsSettings> Settings) : IUserManualCatalog
{
    private readonly string _folder = Settings.Value.Path;

    public Task<IReadOnlyCollection<UserManual>> ListAsync(CancellationToken CancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<UserManual>>(Declared());

    public Task<UserManualFile?> OpenAsync(string Id, CancellationToken CancellationToken = default)
    {
        UserManual? manual = Declared().FirstOrDefault(M => string.Equals(M.Id, Id, StringComparison.Ordinal));
        if (manual is null)
            return Task.FromResult<UserManualFile?>(null);

        try
        {
            Stream content = File.OpenRead(Path.Combine(_folder, manual.FileName));
            return Task.FromResult<UserManualFile?>(new UserManualFile(content, manual));
        }
        catch (Exception Error) when (Error is FileNotFoundException or DirectoryNotFoundException)
        {
            throw UserManualsException.FileMissing(manual.Id, manual.FileName, _folder);
        }
    }

    /// <summary>Every entry is checked before any is returned, so one bad entry fails the list and every download.</summary>
    private List<UserManual> Declared()
    {
        IReadOnlyList<UserManualsManifest.Entry> entries;
        try
        {
            entries = UserManualsManifest.Read(_folder);
        }
        catch (Exception Error) when (Error is FileNotFoundException or DirectoryNotFoundException)
        {
            throw UserManualsException.ManifestMissing(UserManualsManifest.FileName, _folder);
        }
        catch (JsonException Error)
        {
            throw UserManualsException.ManifestInvalid(UserManualsManifest.FileName, $"it is not valid JSON ({Error.Message})");
        }

        var manuals = new List<UserManual>(entries.Count);
        foreach (UserManualsManifest.Entry entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry.Id))
                throw UserManualsException.ManifestInvalid(UserManualsManifest.FileName, "a manual has an empty id.");

            if (!UserManualsManifest.IsPlainFileName(entry.FileName))
            {
                throw UserManualsException.ManifestInvalid(
                    UserManualsManifest.FileName,
                    $"the manual '{entry.Id}': file name '{entry.FileName}' is not a plain file name.");
            }

            if (!UserManualsManifest.TryParseFormat(entry.Format, out UserManualFormat format))
            {
                throw UserManualsException.ManifestInvalid(
                    UserManualsManifest.FileName,
                    $"the manual '{entry.Id}': format '{entry.Format}' is not supported (use pdf or odt).");
            }

            manuals.Add(new UserManual(
                entry.Id,
                entry.Title ?? entry.Id,
                entry.Language ?? string.Empty,
                entry.Version ?? string.Empty,
                entry.FileName!,
                format));
        }

        return manuals;
    }
}
