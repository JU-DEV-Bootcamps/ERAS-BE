namespace Eras.Application.Contracts.Infrastructure;

public interface IFileEncryptionService
{
    Task<Stream> EncryptAsync(Stream plainStream, CancellationToken cancellationToken = default);
    Task<Stream> DecryptAsync(Stream cipherStream, CancellationToken cancellationToken = default);
}