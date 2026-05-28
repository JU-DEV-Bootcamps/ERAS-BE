using System.Security.Cryptography;
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Eras.Infrastructure.FileStorage;

public sealed class AesFileEncryptionService : IFileEncryptionService
{
    private readonly byte[] _key;
    private const int IvSize = 16;

    public AesFileEncryptionService(IConfiguration configuration)
    {
        string? keyBase64 = configuration["Encryption:Key"];

        if (string.IsNullOrEmpty(keyBase64))
            throw new InvalidOperationException("Encryption:Key is not configured.");

        _key = Convert.FromBase64String(keyBase64);
    }

    public async Task<Stream> EncryptAsync(Stream plainStream, CancellationToken cancellationToken = default)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        MemoryStream output = new();

        await output.WriteAsync(aes.IV, cancellationToken);

        await using CryptoStream cryptoStream = new(
            output,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write,
            leaveOpen: true);

        await plainStream.CopyToAsync(cryptoStream, cancellationToken);
        await cryptoStream.FlushFinalBlockAsync(cancellationToken);

        output.Position = 0;
        return output;
    }

    public async Task<Stream> DecryptAsync(Stream cipherStream, CancellationToken cancellationToken = default)
    {
        byte[] iv = new byte[IvSize];
        int bytesRead = await cipherStream.ReadAsync(iv, cancellationToken);

        if (bytesRead != IvSize)
            throw new InvalidDataException("File is missing IV header — may not be encrypted.");

        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;

        MemoryStream output = new();

        await using CryptoStream cryptoStream = new(
            cipherStream,
            aes.CreateDecryptor(),
            CryptoStreamMode.Read,
            leaveOpen: true);

        await cryptoStream.CopyToAsync(output, cancellationToken);

        output.Position = 0;
        return output;
    }
}