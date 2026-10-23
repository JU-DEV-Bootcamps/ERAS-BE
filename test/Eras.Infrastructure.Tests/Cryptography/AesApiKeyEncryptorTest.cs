using System.Security.Cryptography;
using System.Text;
using Eras.Domain.Common;
using Microsoft.Extensions.Configuration;
using Eras.Infrastructure.Cryptography;

using Xunit;

namespace Eras.Infrastructure.Tests.Cryptography;

public class AesApiKeyEncryptorTests
{
    private const string KeyHex = "00112233445566778899AABBCCDDEEFF00112233445566778899AABBCCDDEEFF";
    private const string IVHex = "00112233445566778899AABBCCDDEEFF";

    private static IConfiguration CreateConfiguration(string? key = KeyHex, string? iv = IVHex)
    {
        var values = new Dictionary<string, string?>();

        if (key != null)
        {
            values["Encryption:Key"] = key;
        }

        if (iv != null)
        {
            values["Encryption:IV"] = iv;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    [Fact]
    public void Constructor_WhenConfigurationIsValid_CreatesEncryptor()
    {
        // Act
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());

        // Assert
        Assert.NotNull(encryptor);
    }

    [Fact]
    public void Constructor_WhenKeyIsMissing_ThrowsException()
    {
        // Arrange
        var configuration = CreateConfiguration(key: null);

        // Act
        var exception = Assert.Throws<Exception>(
            () => new AesApiKeyEncryptor(configuration));

        // Assert
        Assert.Equal("Key not found", exception.Message);
    }

    [Fact]
    public void Constructor_WhenIVIsMissing_ThrowsException()
    {
        // Arrange
        var configuration = CreateConfiguration(iv: null);

        // Act
        var exception = Assert.Throws<Exception>(
            () => new AesApiKeyEncryptor(configuration));

        // Assert
        Assert.Equal("IV not found", exception.Message);
    }

    [Fact]
    public void Encrypt_WhenPlainTextIsProvided_ReturnsBase64CipherText()
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());
        const string plainText = "my-api-key";

        // Act
        var cipherText = encryptor.Encrypt(plainText);

        // Assert
        Assert.NotNull(cipherText);
        Assert.NotEmpty(cipherText);

        var exception = Record.Exception(
            () => Convert.FromBase64String(cipherText));

        Assert.Null(exception);
    }

    [Fact]
    public void EncryptThenDecrypt_ReturnsOriginalPlainText()
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());
        const string plainText = "my-secret-api-key";

        // Act
        var cipherText = encryptor.Encrypt(plainText);
        var decryptedText = encryptor.Decrypt(cipherText);

        // Assert
        Assert.Equal(plainText, decryptedText);
    }

    [Theory]
    [InlineData("")]
    [InlineData("hello")]
    [InlineData("my-api-key-123")]
    [InlineData("API_KEY_WITH_SPECIAL_CHARS!@#$%^&*()")]
    [InlineData("こんにちは世界")]
    [InlineData("secret key")]
    public void EncryptThenDecrypt_WithVariousPlainTexts_ReturnsOriginalText(
        string plainText)
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());

        // Act
        var cipherText = encryptor.Encrypt(plainText);
        var decryptedText = encryptor.Decrypt(cipherText);

        // Assert
        Assert.Equal(plainText, decryptedText);
    }

    [Fact]
    public void Encrypt_WithSamePlainTextAndConfiguration_ReturnsSameCipherText()
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());
        const string plainText = "my-api-key";

        // Act
        var firstCipherText = encryptor.Encrypt(plainText);
        var secondCipherText = encryptor.Encrypt(plainText);

        // Assert
        Assert.Equal(firstCipherText, secondCipherText);
    }

    [Fact]
    public void Decrypt_WhenCipherTextIsInvalidBase64_ThrowsFormatException()
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());

        // Assert
        Assert.Throws<FormatException>(
            () => encryptor.Decrypt("not-valid-base64!!!"));
    }

    [Fact]
    public void Decrypt_WhenCipherTextIsInvalid_ThrowsCryptographicException()
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());

        // Valid Base64
        var invalidCipherText = Convert.ToBase64String(
            new byte[] { 1, 2, 3, 4, 5 });

        // Assert
        Assert.ThrowsAny<CryptographicException>(
            () => encryptor.Decrypt(invalidCipherText));
    }

    [Fact]
    public void Decrypt_WhenCipherTextWasEncryptedWithDifferentKey_ThrowsCryptographicException()
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());

        var otherEncryptor = new AesApiKeyEncryptor(
            CreateConfiguration(
                key: "FFEEDDCCBBAA99887766554433221100FFEEDDCCBBAA99887766554433221100"));

        var cipherText = otherEncryptor.Encrypt("my-api-key");

        // Assert
        Assert.ThrowsAny<CryptographicException>(
            () => encryptor.Decrypt(cipherText));
    }

    [Fact]
    public void Encrypt_WhenPlainTextIsLong_ReturnsDecryptableCipherText()
    {
        // Arrange
        var encryptor = new AesApiKeyEncryptor(CreateConfiguration());
        var plainText = new string('A', 10_000);

        // Act
        var cipherText = encryptor.Encrypt(plainText);
        var decryptedText = encryptor.Decrypt(cipherText);

        // Assert
        Assert.Equal(plainText, decryptedText);
    }
}
