using Eras.Application.Contracts.Infrastructure;
using Eras.Infrastructure.FileStorage;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Infrastructure.Tests.FileStorage;

/// <summary>
/// Runs the shared <see cref="FileStorageServiceContractTests"/> suite against
/// <see cref="LocalFileStorageService"/> backed by a real, disposable temp directory on disk.
/// xUnit constructs a fresh instance of this class per test method, so each test gets its own
/// temp directory, cleaned up in <see cref="Dispose"/>.
/// </summary>
public sealed class LocalFileStorageServiceContractTests : FileStorageServiceContractTests, IDisposable
{
    private readonly DirectoryInfo _tempDirectory =
        Directory.CreateTempSubdirectory("eras-filestorage-contract-");

    protected override IFileStorageService CreateSut()
    {
        return new LocalFileStorageService(
            _tempDirectory.FullName,
            new PassThroughFileEncryptionService(),
            Mock.Of<ILogger<LocalFileStorageService>>());
    }

    public void Dispose()
    {
        try
        {
            _tempDirectory.Delete(recursive: true);
        }
        catch (IOException)
        {
            // Best-effort cleanup; a leftover temp dir doesn't fail the test.
        }
    }
}
