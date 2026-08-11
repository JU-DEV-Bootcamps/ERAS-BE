using Eras.Application.Contracts.Infrastructure;

namespace Eras.Infrastructure.Tests.FileStorage;

/// <summary>
/// No-op <see cref="IFileEncryptionService"/> fake used by <see cref="FileStorageServiceContractTests"/>
/// so the storage-provider contract can be exercised in isolation from AES specifics (which have
/// their own concern and aren't part of what the contract suite verifies).
/// </summary>
internal sealed class PassThroughFileEncryptionService : IFileEncryptionService
{
    public async Task<Stream> EncryptAsync(Stream plainStream, CancellationToken cancellationToken = default)
    {
        var output = new MemoryStream();
        await plainStream.CopyToAsync(output, cancellationToken);
        output.Position = 0;
        return output;
    }

    public async Task<Stream> DecryptAsync(Stream cipherStream, CancellationToken cancellationToken = default)
    {
        var output = new MemoryStream();
        await cipherStream.CopyToAsync(output, cancellationToken);
        output.Position = 0;
        return output;
    }
}
