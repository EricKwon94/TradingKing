using System;
using System.Security.Cryptography;
using System.Text;

namespace Common;

public class Encryptor
{
    public string Encrypt(string plainText)
    {
        byte[] encrypt = SHA256.HashData(Encoding.UTF8.GetBytes(plainText));
        return Convert.ToHexString(encrypt);
    }
}
