using System.Text;

using Eras.Application.Contracts.Infrastructure;

namespace Eras.Infrastructure.Tests.FileStorage;

/// <summary>
/// Reusable contract suite for any <see cref="IFileStorageService"/> implementation. Subclass and
/// implement <see cref="CreateSut"/> to hold a new provider (e.g. a future Swift provider) to the
/// same behavioral bar as <see cref="LocalFileStorageServiceContractTests"/> — every provider must
/// pass this suite unmodified.
/// </summary>
public abstract class FileStorageServiceContractTests
{
    /// <summary>Creates a fresh instance of the provider under test.</summary>
    protected abstract IFileStorageService CreateSut();

    private static Stream ContentStream(string content) => new MemoryStream(Encoding.UTF8.GetBytes(content));

    private static async Task<string> ReadAllTextAsync(Stream stream)
    {
        await using var _ = stream;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    [Fact]
    public async Task SaveAsync_Should_ReturnKey_ThatReadAsyncRetrievesTheSameContentFrom()
    {
        IFileStorageService sut = CreateSut();

        string key = await sut.SaveAsync(ContentStream("hello world"), "file.txt", "contract-tests");
        string content = await ReadAllTextAsync(await sut.ReadAsync(key));

        Assert.Equal("hello world", content);
    }

    [Fact]
    public async Task SaveAsync_Should_NotPreserveOriginalFileName_InTheReturnedKey()
    {
        IFileStorageService sut = CreateSut();

        string key = await sut.SaveAsync(ContentStream("data"), "very-original-name.pdf", "contract-tests");

        Assert.DoesNotContain("very-original-name", key);
        Assert.EndsWith(".pdf", key);
    }

    [Fact]
    public async Task ReadAsync_Should_ThrowFileNotFoundException_ForAnUnknownKey()
    {
        IFileStorageService sut = CreateSut();

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => sut.ReadAsync("contract-tests/does-not-exist.bin"));
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnTrue_AfterSaveAsync()
    {
        IFileStorageService sut = CreateSut();

        string key = await sut.SaveAsync(ContentStream("data"), "file.bin", "contract-tests");

        Assert.True(await sut.ExistsAsync(key));
    }

    [Fact]
    public async Task ExistsAsync_Should_ReturnFalse_ForAnUnknownKey()
    {
        IFileStorageService sut = CreateSut();

        Assert.False(await sut.ExistsAsync("contract-tests/does-not-exist.bin"));
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveTheFile_SoExistsAsyncThenReturnsFalse()
    {
        IFileStorageService sut = CreateSut();

        string key = await sut.SaveAsync(ContentStream("data"), "file.bin", "contract-tests");
        await sut.DeleteAsync(key);

        Assert.False(await sut.ExistsAsync(key));
    }

    [Fact]
    public async Task DeleteAsync_Should_NotThrow_ForAnUnknownKey()
    {
        IFileStorageService sut = CreateSut();

        await sut.DeleteAsync("contract-tests/does-not-exist.bin");
    }

    [Fact]
    public async Task GetUrlAsync_Should_NotThrow_AndReturnEitherNullOrAnAbsoluteUri()
    {
        IFileStorageService sut = CreateSut();
        string key = await sut.SaveAsync(ContentStream("data"), "file.bin", "contract-tests");

        string? url = await sut.GetUrlAsync(key);

        Assert.True(url is null || Uri.IsWellFormedUriString(url, UriKind.Absolute));
    }

    [Fact]
    public async Task GetUrlAsync_Should_NotThrow_ForAnUnknownKey()
    {
        IFileStorageService sut = CreateSut();

        string? url = await sut.GetUrlAsync("contract-tests/does-not-exist.bin");

        Assert.True(url is null || Uri.IsWellFormedUriString(url, UriKind.Absolute));
    }
}
