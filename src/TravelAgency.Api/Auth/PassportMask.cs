using System.Security.Cryptography;

using TravelAgency.Core.Interfaces;

namespace TravelAgency.Api.Auth;

/// <summary>Маскування паспортних даних для кабінету менеджера: повний номер у відповідях API не повертається.</summary>
public static class PassportMask
{
    /// <summary>Розшифровує значення з бази та маскує його (наприклад, AB123456 → AB****56).</summary>
    public static string FromEncrypted(IEncryptionService encryption, string cipherText)
    {
        try
        {
            return Mask(encryption.Decrypt(cipherText));
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException)
        {
            // Значення не вдалося розшифрувати (наприклад, змінено ключ) — нічого не розкриваємо
            return "****";
        }
    }

    /// <summary>Залишає по два символи на початку й наприкінці; короткі значення маскуються повністю.</summary>
    public static string Mask(string passport)
    {
        var value = passport.Trim();

        if (value.Length <= 4)
            return new string('*', Math.Max(value.Length, 4));

        return value[..2] + new string('*', value.Length - 4) + value[^2..];
    }
}
