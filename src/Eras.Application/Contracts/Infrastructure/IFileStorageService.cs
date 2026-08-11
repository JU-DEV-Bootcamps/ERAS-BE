namespace Eras.Application.Contracts.Infrastructure;

/// <summary>
/// Generic, entity-agnostic contract for storing and retrieving file content. 
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves <paramref name="fileStream"/> under <paramref name="folder"/>, generating a
    /// collision-safe physical name derived from <paramref name="fileName"/>'s extension
    /// (the original name itself is not preserved by the storage layer — callers that need it
    /// back must persist it separately, e.g. in <c>Attachment.OriginalFileName</c>).
    /// </summary>
    /// <param name="fileStream">The file content to persist. Read to completion by this call.</param>
    /// <param name="fileName">Original file name; only its extension is used.</param>
    /// <param name="folder">Logical folder/partition the file is stored under.</param>
    /// <returns>
    /// The storage key identifying the saved file — pass this back to <see cref="ReadAsync"/>,
    /// <see cref="DeleteAsync"/>, <see cref="ExistsAsync"/>, and <see cref="GetUrlAsync"/>.
    /// </returns>
    Task<string> SaveAsync(Stream fileStream, string fileName, string folder);

    /// <summary>
    /// Opens the file identified by <paramref name="key"/> for reading.
    /// </summary>
    /// <param name="key">The storage key previously returned by <see cref="SaveAsync"/>.</param>
    /// <returns>A readable stream positioned at the start of the file's (decrypted) content.</returns>
    /// <exception cref="FileNotFoundException">No file exists for <paramref name="key"/>.</exception>
    Task<Stream> ReadAsync(string key);

    /// <summary>
    /// Deletes the file identified by <paramref name="key"/>, if it exists.
    /// </summary>
    /// <param name="key">The storage key previously returned by <see cref="SaveAsync"/>.</param>
    /// <remarks>Idempotent: deleting a key that does not exist is not an error.</remarks>
    Task DeleteAsync(string key);

    /// <summary>
    /// Checks whether a file exists for <paramref name="key"/>, without reading its content.
    /// </summary>
    /// <param name="key">The storage key previously returned by <see cref="SaveAsync"/>.</param>
    /// <returns><see langword="true"/> if the file is present; otherwise <see langword="false"/>. Never throws for a missing key.</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Returns a URL clients can use to fetch the file identified by <paramref name="key"/>
    /// directly from the storage backend (e.g. a signed, time-limited Swift Temporary URL),
    /// bypassing the application server.
    /// </summary>
    /// <param name="key">The storage key previously returned by <see cref="SaveAsync"/>.</param>
    /// <returns>
    /// The direct-access URL, or <see langword="null"/> if this provider has no such concept
    /// (e.g. local disk — callers must fall back to streaming the file via <see cref="ReadAsync"/>).
    /// Does not throw for a missing key; callers should still check <see cref="ExistsAsync"/> or
    /// handle a failed download at the returned URL.
    /// </returns>
    Task<string?> GetUrlAsync(string key);
}
