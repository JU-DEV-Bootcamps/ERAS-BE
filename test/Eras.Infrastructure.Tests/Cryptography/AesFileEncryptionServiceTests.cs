using System.Security.Cryptography;
using Eras.Infrastructure.FileStorage;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Eras.Infrastructure.Tests.Cryptography;

public class AesFileEncryptionServiceTests
{
    private const string ValidHexKey =
        "00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF";

    private static AesFileEncryptionService CreateService(string? key = ValidHexKey)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:Key"] = key
            })
            .Build();

        return new AesFileEncryptionService(configuration);
    }

    private static MemoryStream CreateStream(string content)
    {
        return new MemoryStream(
            System.Text.Encoding.UTF8.GetBytes(content));
    }

    private static async Task<string> ReadStreamAsStringAsync(Stream stream)
    {
        stream.Position = 0;

        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }

    [Fact]
    public void Constructor_WhenEncryptionKeyIsMissing_ThrowsInvalidOperationException()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => new AesFileEncryptionService(configuration));

        Assert.Equal(
            "Encryption:Key is not configured.",
            exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WhenEncryptionKeyIsEmpty_ThrowsInvalidOperationException(
        string? key)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:Key"] = key
            })
            .Build();

        Assert.Throws<InvalidOperationException>(
            () => new AesFileEncryptionService(configuration));
    }

    [Fact]
    public void Constructor_WhenEncryptionKeyIsInvalidHex_ThrowsFormatException()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:Key"] = "not-a-valid-hex-key"
            })
            .Build();

        Assert.Throws<FormatException>(
            () => new AesFileEncryptionService(configuration));
    }

    [Fact]
    public async Task EncryptAsync_WritesIvAtBeginningOfStream()
    {
        AesFileEncryptionService service = CreateService();
        using MemoryStream plainStream = CreateStream("Hello, World!");

        using Stream encryptedStream =
            await service.EncryptAsync(plainStream);

        Assert.True(encryptedStream.Length >= 16);

        byte[] iv = new byte[16];

        encryptedStream.Position = 0;
        int bytesRead = await encryptedStream.ReadAsync(iv);

        Assert.Equal(16, bytesRead);
        Assert.False(iv.All(b => b == 0));
    }

    [Fact]
    public async Task EncryptAsync_GeneratesDifferentIvForEachEncryption()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream plainStream1 = CreateStream("Same content");
        using MemoryStream plainStream2 = CreateStream("Same content");

        using Stream encryptedStream1 =
            await service.EncryptAsync(plainStream1);

        using Stream encryptedStream2 =
            await service.EncryptAsync(plainStream2);

        byte[] iv1 = new byte[16];
        byte[] iv2 = new byte[16];

        encryptedStream1.Position = 0;
        encryptedStream2.Position = 0;

        await encryptedStream1.ReadAsync(iv1);
        await encryptedStream2.ReadAsync(iv2);

        Assert.NotEqual(iv1, iv2);
    }

    [Fact]
    public async Task EncryptAsync_AndDecryptAsync_ReturnsOriginalContent()
    {
        AesFileEncryptionService service = CreateService();

        const string originalContent = "This is sensitive file content.";
        using MemoryStream plainStream = CreateStream(originalContent);
        using Stream encryptedStream = await service.EncryptAsync(plainStream);
        using Stream decryptedStream = await service.DecryptAsync(encryptedStream);
        string decryptedContent = await ReadStreamAsStringAsync(decryptedStream);

        Assert.Equal(originalContent, decryptedContent);
    }

    [Fact]
    public async Task EncryptAsync_AndDecryptAsync_WorksWithEmptyStream()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream plainStream = new();
        using Stream encryptedStream = await service.EncryptAsync(plainStream);
        using Stream decryptedStream = await service.DecryptAsync(encryptedStream);
        string decryptedContent = await ReadStreamAsStringAsync(decryptedStream);

        Assert.Empty(decryptedContent);
    }

    [Fact]
    public async Task EncryptAsync_AndDecryptAsync_WorksWithLargeStream()
    {
        AesFileEncryptionService service = CreateService();

        byte[] data = new byte[1024 * 1024];
        RandomNumberGenerator.Fill(data);
        using MemoryStream plainStream = new(data);
        using Stream encryptedStream = await service.EncryptAsync(plainStream);
        using Stream decryptedStream = await service.DecryptAsync(encryptedStream);
        using MemoryStream result = new();
        await decryptedStream.CopyToAsync(result);

        Assert.Equal(data, result.ToArray());
    }

    [Fact]
    public async Task DecryptAsync_WhenStreamDoesNotContainIv_ThrowsInvalidDataException()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream cipherStream = new(new byte[10]);

        InvalidDataException exception = await Assert.ThrowsAsync<InvalidDataException>(
                () => service.DecryptAsync(cipherStream));

        Assert.Equal("File is missing IV header — may not be encrypted.", exception.Message);
    }

    [Fact]
    public async Task DecryptAsync_WhenStreamIsEmpty_ThrowsInvalidDataException()
    {
        AesFileEncryptionService service = CreateService();
        using MemoryStream cipherStream = new();

        await Assert.ThrowsAsync<InvalidDataException>(
            () => service.DecryptAsync(cipherStream));
    }

    [Fact]
    public async Task DecryptAsync_WhenCipherTextIsCorrupted_ThrowsCryptographicException()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream plainStream = CreateStream("Sensitive information");
        using Stream encryptedStream = await service.EncryptAsync(plainStream);
        byte[] encryptedData;
        using (MemoryStream temp = new())
        {
            await encryptedStream.CopyToAsync(temp);
            encryptedData = temp.ToArray();
        }
        encryptedData[^1] ^= 0xFF;
        using MemoryStream corruptedStream = new(encryptedData);

        await Assert.ThrowsAnyAsync<CryptographicException>(
            () => service.DecryptAsync(corruptedStream));
    }

    [Fact]
    public async Task DecryptAsync_WhenWrongKeyIsUsed_ThrowsCryptographicException()
    {
        AesFileEncryptionService encryptionService = CreateService(ValidHexKey);
        const string differentKey = "FFEEDDCCBBAA99887766554433221100FFEEDDCCBBAA99887766554433221100";
        AesFileEncryptionService decryptionService = CreateService(differentKey);
        using MemoryStream plainStream = CreateStream("Sensitive information");
        using Stream encryptedStream = await encryptionService.EncryptAsync(plainStream);

        await Assert.ThrowsAnyAsync<CryptographicException>(
            () => decryptionService.DecryptAsync(encryptedStream));
    }

    [Fact]
    public async Task DecryptAsync_DoesNotCloseInputStream()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream plainStream = CreateStream("Hello");
        Stream encryptedStream = await service.EncryptAsync(plainStream);
        using Stream decryptedStream = await service.DecryptAsync(encryptedStream);

        Assert.True(encryptedStream.CanRead);
    }

    [Fact]
    public async Task EncryptAsync_DoesNotCloseInputStream()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream plainStream = CreateStream("Hello");
        Stream encryptedStream = await service.EncryptAsync(plainStream);

        Assert.True(plainStream.CanRead);
        await encryptedStream.DisposeAsync();
    }

    [Fact]
    public async Task EncryptAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream plainStream = CreateStream("Some content");
        using CancellationTokenSource cts = new();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.EncryptAsync(plainStream, cts.Token));
    }

    [Fact]
    public async Task DecryptAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        AesFileEncryptionService service = CreateService();

        using MemoryStream plainStream = CreateStream("Some content");
        using Stream encryptedStream = await service.EncryptAsync(plainStream);
        using CancellationTokenSource cts = new();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.DecryptAsync(encryptedStream, cts.Token));
    }
}
