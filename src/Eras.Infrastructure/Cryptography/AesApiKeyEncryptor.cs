using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Infrastructure.Cryptography;
public class AesApiKeyEncryptor : IApiKeyEncryptor
{

    private static readonly byte[] Key = Convert.FromHexString("c0427fcd4880380275616fbb3019c339584defa39cbb0f7185a29dabe9befa9d");
    private static readonly byte[] IV = Convert.FromHexString("4d59d9b834ccbf185d29922fbeec2f4c");

    public string Decrypt(string CipherText)
    {

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(CipherText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();

    }
    public string Encrypt(string PlainText)
    {

        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

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
