using TravelAgency.Core.Entities;

namespace TravelAgency.Api.Auth;

/// <summary>Випуск JWT-токенів для автентифікованих користувачів (UC-02).</summary>
public interface ITokenService
{
    /// <summary>Створює підписаний токен і повертає його разом із часом завершення дії (UTC).</summary>
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
