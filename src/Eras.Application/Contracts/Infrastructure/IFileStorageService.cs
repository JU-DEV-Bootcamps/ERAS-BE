namespace Eras.Application.Contracts.Infrastructure;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream fileStream, string fileName, string folder);
    Task DeleteAsync(string relativePath);
}