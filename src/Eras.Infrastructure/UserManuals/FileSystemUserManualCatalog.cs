using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Models;
using Eras.Domain.Entities;

using Microsoft.Extensions.Options;

namespace Eras.Infrastructure.UserManuals;

/// <summary>Reads the manuals declared in <c>manifest.json</c> from the configured manuals folder.</summary>
public sealed class FileSystemUserManualCatalog(IOptions<UserManualsSettings> Settings) : IUserManualCatalog
{
    private readonly string _folder = Settings.Value.Path;

    public Task<IReadOnlyCollection<UserManual>> ListAsync(CancellationToken CancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<UserManual>>(Declared().ToList());

    public Task<UserManualFile?> OpenAsync(string Id, CancellationToken CancellationToken = default)
    {
        UserManual? manual = Declared().FirstOrDefault(M => string.Equals(M.Id, Id, StringComparison.Ordinal));
        if (manual is null)
            return Task.FromResult<UserManualFile?>(null);

        Stream content = File.OpenRead(Path.Combine(_folder, manual.FileName));
        return Task.FromResult<UserManualFile?>(new UserManualFile(content, manual));
    }

    private IEnumerable<UserManual> Declared()
    {
        foreach (UserManualsManifest.Entry entry in UserManualsManifest.Read(_folder))
        {
            if (string.IsNullOrWhiteSpace(entry.Id)
                || !UserManualsManifest.IsPlainFileName(entry.FileName)
                || !UserManualsManifest.TryParseFormat(entry.Format, out UserManualFormat format))
            {
                throw new InvalidDataException($"The manual '{entry.Id}' in {UserManualsManifest.FileName} is not valid.");
            }

            yield return new UserManual(
                entry.Id,
                entry.Title ?? entry.Id,
                entry.Language ?? string.Empty,
                entry.Version ?? string.Empty,
                entry.FileName!,
                format);
        }
    }
}
