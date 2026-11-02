using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure;

public sealed record UserManualFile(Stream Content, UserManual Manual);

/// <summary>Read-only access to the manuals declared in the repository's manuals folder.</summary>
public interface IUserManualCatalog
{
    Task<IReadOnlyCollection<UserManual>> ListAsync(CancellationToken CancellationToken = default);

    /// <returns>The manual and its file content, or null when no manual is declared with <paramref name="Id"/>.</returns>
    Task<UserManualFile?> OpenAsync(string Id, CancellationToken CancellationToken = default);
}
