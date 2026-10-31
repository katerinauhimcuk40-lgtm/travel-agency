using TravelAgency.Core.Entities;

namespace TravelAgency.Core.Interfaces;

/// <summary>Реєстрація та автентифікація користувачів (UC-01, UC-02).</summary>
public interface IAuthService
{
    /// <summary>Створює користувача з роллю Client. Повторна пошта — InvalidOperationException.</summary>
    Task<User> RegisterAsync(string email, string password, string fullName, string? phone);

    /// <summary>Перевіряє пошту й пароль. Невірні дані — UnauthorizedAccessException.</summary>
    Task<User> LoginAsync(string email, string password);
}
