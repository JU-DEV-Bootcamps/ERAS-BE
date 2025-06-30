using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Common;
public interface IApiKeyEncryptor
{
    string Encrypt(string PlainText);
    string Decrypt(string CipherText);

}
