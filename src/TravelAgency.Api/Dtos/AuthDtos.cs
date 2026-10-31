using System.ComponentModel.DataAnnotations;

using TravelAgency.Core.Enums;

namespace TravelAgency.Api.Dtos;

public class RegisterRequest
{
    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }
}

public class LoginRequest
{
    [Required, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Відповідь на вхід або реєстрацію: токен для заголовка Authorization: Bearer і дані користувача.</summary>
public record AuthResponse(string Token, DateTime ExpiresAt, int UserId, string Email, string FullName, Role Role);
