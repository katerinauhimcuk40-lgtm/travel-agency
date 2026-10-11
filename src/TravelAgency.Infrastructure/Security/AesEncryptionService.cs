using System.Security.Cryptography;
using System.Text;

using TravelAgency.Core.Interfaces;

namespace TravelAgency.Infrastructure.Security;

/// <summary>
/// Шифрування AES-256-GCM. Формат результату (Base64): nonce (12 байт) + tag (16 байт) + шифртекст.
/// Для кожного виклику Encrypt генерується новий nonce, тому однаковий текст дає різні шифртексти.
/// </summary>
public sealed class AesEncryptionService : IEncryptionService
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private readonly byte[] _key;

    public AesEncryptionService(string base64Key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(base64Key);

        _key = Convert.FromBase64String(base64Key);
        if (_key.Length != 32)
            throw new ArgumentException("Encryption key must be 32 bytes (256 bit), Base64-encoded.", nameof(base64Key));
    }

    public string Encrypt(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText);

        var plain = Encoding.UTF8.GetBytes(plainText);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var cipher = new byte[plain.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plain, cipher, tag);

        var result = new byte[NonceSize + TagSize + cipher.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
        Buffer.BlockCopy(cipher, 0, result, NonceSize + TagSize, cipher.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        ArgumentNullException.ThrowIfNull(cipherText);

        var data = Convert.FromBase64String(cipherText);
        if (data.Length < NonceSize + TagSize)
            throw new CryptographicException("Invalid cipher text.");

        var nonce = data.AsSpan(0, NonceSize);
        var tag = data.AsSpan(NonceSize, TagSize);
        var cipher = data.AsSpan(NonceSize + TagSize);
        var plain = new byte[cipher.Length];

        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipher, tag, plain);

        return Encoding.UTF8.GetString(plain);
    }
}
