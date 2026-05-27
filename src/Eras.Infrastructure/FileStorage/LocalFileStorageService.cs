using Eras.Application.Contracts.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Eras.Infrastructure.FileStorage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(string basePath, ILogger<LocalFileStorageService> logger)
    {
        _basePath = basePath;
        _logger = logger;
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName, string folder)
    {
        string safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        string directory = Path.Combine(_basePath, folder);
        Directory.CreateDirectory(directory);

        string fullPath = Path.Combine(directory, safeFileName);
        await using FileStream fs = File.Create(fullPath);
        await fileStream.CopyToAsync(fs);

        _logger.LogInformation("File saved: {Path}", fullPath);

        return Path.Combine(folder, safeFileName).Replace('\\', '/');
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