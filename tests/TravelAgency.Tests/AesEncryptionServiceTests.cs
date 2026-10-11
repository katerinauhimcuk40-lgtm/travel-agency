using System.Security.Cryptography;

using TravelAgency.Infrastructure.Security;

namespace TravelAgency.Tests;

public class AesEncryptionServiceTests
{
    private static string NewKey() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    [Fact]
    public void EncryptDecrypt_RoundTrip_ReturnsOriginal()
    {
        var service = new AesEncryptionService(NewKey());

        var decrypted = service.Decrypt(service.Encrypt("AB123456"));

        Assert.Equal("AB123456", decrypted);
    }

    [Fact]
    public void EncryptDecrypt_UnicodeText_ReturnsOriginal()
    {
        var service = new AesEncryptionService(NewKey());

        var decrypted = service.Decrypt(service.Encrypt("Паспорт АБ №123456"));

        Assert.Equal("Паспорт АБ №123456", decrypted);
    }

    [Fact]
    public void Encrypt_SameTextTwice_GivesDifferentCipherTexts()
    {
        var service = new AesEncryptionService(NewKey());

        var first = service.Encrypt("AB123456");
        var second = service.Encrypt("AB123456");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Encrypt_DoesNotContainPlainText()
    {
        var service = new AesEncryptionService(NewKey());

        var cipher = service.Encrypt("AB123456");

        Assert.DoesNotContain("AB123456", cipher);
    }

    [Fact]
    public void Decrypt_TamperedCipherText_Throws()
    {
        var service = new AesEncryptionService(NewKey());
        var data = Convert.FromBase64String(service.Encrypt("AB123456"));
        data[^1] ^= 0xFF;

        Assert.ThrowsAny<CryptographicException>(() => service.Decrypt(Convert.ToBase64String(data)));
    }

    [Fact]
    public void Decrypt_WithWrongKey_Throws()
    {
        var cipher = new AesEncryptionService(NewKey()).Encrypt("AB123456");
        var otherService = new AesEncryptionService(NewKey());

        Assert.ThrowsAny<CryptographicException>(() => otherService.Decrypt(cipher));
    }

    [Fact]
    public void Decrypt_TooShortData_Throws()
    {
        var service = new AesEncryptionService(NewKey());

        Assert.ThrowsAny<CryptographicException>(() => service.Decrypt(Convert.ToBase64String(new byte[5])));
    }

    [Fact]
    public void Constructor_KeyWithWrongLength_Throws()
    {
        var shortKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        Assert.Throws<ArgumentException>(() => new AesEncryptionService(shortKey));
    }
}
