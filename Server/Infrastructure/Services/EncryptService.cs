using Application.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

internal class EncryptService : IEncryptService
{
    public string Encrypt(string plainText)
    {
        byte[] encrypt = SHA256.HashData(Encoding.UTF8.GetBytes(plainText));
        return Convert.ToHexString(encrypt);
    }
}
