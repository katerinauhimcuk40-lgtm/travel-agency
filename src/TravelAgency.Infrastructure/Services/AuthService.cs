using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;
using TravelAgency.Core.Interfaces;

namespace TravelAgency.Infrastructure.Services;

/// <summary>Реєстрація та вхід. Паролі зберігаються лише як хеші BCrypt.</summary>
public class AuthService : IAuthService
{
    public const int MinPasswordLength = 8;

    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User> RegisterAsync(string email, string password, string fullName, string? phone)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength)
            throw new ArgumentException($"Password must be at least {MinPasswordLength} characters long.", nameof(password));

        var normalizedEmail = NormalizeEmail(email);

        var existing = await _unitOfWork.Users.FindAsync(u => u.Email == normalizedEmail);
        if (existing.Any())
            throw new InvalidOperationException("User with this email already exists.");

        var user = new User
        {
            Email = normalizedEmail,
            FullName = fullName.Trim(),
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Role = Role.Client,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var normalizedEmail = NormalizeEmail(email ?? string.Empty);

        var user = (await _unitOfWork.Users.FindAsync(u => u.Email == normalizedEmail)).FirstOrDefault();

        if (user is null || !user.IsActive || !VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return user;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static bool VerifyPassword(string? password, string hash)
    {
        // Хеш "!" (заглушка з початкових даних) та порожні значення не є валідними хешами BCrypt
        if (string.IsNullOrEmpty(password) || !hash.StartsWith("$2", StringComparison.Ordinal))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
