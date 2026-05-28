using Eras.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Eras.Infrastructure.FileStorage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly IFileEncryptionService _encryption;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        string basePath,
        IFileEncryptionService encryption,
        ILogger<LocalFileStorageService> logger)
    {
        _basePath = basePath;
        _encryption = encryption;
        _logger = logger;
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName, string folder)
    {
        string safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        string directory = Path.Combine(_basePath, folder);
        Directory.CreateDirectory(directory);

        if (OperatingSystem.IsLinux())
        {
            File.SetUnixFileMode(directory,
                UnixFileMode.UserRead |
                UnixFileMode.UserWrite |
                UnixFileMode.UserExecute);
        }

        string fullPath = Path.Combine(directory, safeFileName);

        await using Stream encryptedStream = await _encryption.EncryptAsync(fileStream);
        await using FileStream fs = File.Create(fullPath);
        await encryptedStream.CopyToAsync(fs);

        if (OperatingSystem.IsLinux())
        {
            File.SetUnixFileMode(fullPath,
                UnixFileMode.UserRead |
                UnixFileMode.UserWrite);
        }

        _logger.LogInformation("File saved (encrypted): {Path}", fullPath);
        return Path.Combine(folder, safeFileName).Replace('\\', '/');
    }

    public async Task<Stream> ReadAsync(string relativePath)
    {
        string fullPath = Path.Combine(_basePath, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Attachment not found.", fullPath);

        await using FileStream fs = File.OpenRead(fullPath);
        Stream decrypted = await _encryption.DecryptAsync(fs);

        _logger.LogInformation("File read (decrypted): {Path}", fullPath);
        return decrypted;
    }

    public Task DeleteAsync(string relativePath)
    {
        string fullPath = Path.Combine(_basePath, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("File deleted: {Path}", fullPath);
        }
        return Task.CompletedTask;
    }
}