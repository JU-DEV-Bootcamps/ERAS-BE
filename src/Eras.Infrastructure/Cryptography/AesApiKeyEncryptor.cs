using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;
using Microsoft.Extensions.Configuration;

namespace Eras.Infrastructure.Cryptography;
public class AesApiKeyEncryptor : IApiKeyEncryptor
{

    private readonly byte[] _key;
    private readonly byte[] _IV;

    public AesApiKeyEncryptor(IConfiguration configuration)
    {

        var keyHex = configuration.GetSection("Encryption:Key").Value ?? throw new Exception("Key not found");
        _key = Convert.FromHexString(keyHex);
        var IVHex = configuration.GetSection("Encryption:IV").Value ?? throw new Exception("IV not found");
        _IV = Convert.FromHexString(IVHex);
    }

    public string Decrypt(string CipherText)
    {

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _IV;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(CipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();

    }
    public string Encrypt(string PlainText)
    {

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(PlainText);
        }

        return Convert.ToBase64String(ms.ToArray());

    }
}
