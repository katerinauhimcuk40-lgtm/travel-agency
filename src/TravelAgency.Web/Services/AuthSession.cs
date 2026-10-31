using System.Security.Cryptography;

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

using TravelAgency.Core.Enums;
using TravelAgency.Web.Models;

namespace TravelAgency.Web.Services;

/// <summary>Дані поточної сесії користувача (JWT і відомості з відповіді API).</summary>
public sealed record StoredSession(string Token, DateTime ExpiresAt, int UserId, string Email, string FullName, Role Role);

/// <summary>
/// Зберігає JWT поточного користувача в ProtectedSessionStorage (шифрується Data Protection, живе до закриття вкладки).
/// Служба створюється окремо для кожного підключення Blazor (scoped).
/// </summary>
public sealed class AuthSession
{
    private const string StorageKey = "travelagency.session";

    private readonly ProtectedSessionStorage _storage;
    private StoredSession? _current;
    private bool _loaded;

    public AuthSession(ProtectedSessionStorage storage)
    {
        _storage = storage;
    }

    /// <summary>Виникає після входу або виходу.</summary>
    public event Action? Changed;

    public async Task<StoredSession?> GetAsync()
    {
        if (!_loaded)
        {
            try
            {
                var result = await _storage.GetAsync<StoredSession>(StorageKey);
                _current = result.Success ? result.Value : null;
                _loaded = true;
            }
            catch (InvalidOperationException)
            {
                // JS ще недоступний (наприклад, під час prerender) — повернемо порожню сесію й спробуємо пізніше
                return null;
            }
            catch (Exception ex) when (ex is JSException or CryptographicException)
            {
                // Пошкоджене значення в сховищі — вважаємо, що користувач не увійшов
                _current = null;
                _loaded = true;
            }
        }

        if (_current is not null && _current.ExpiresAt <= DateTime.UtcNow)
        {
            await SignOutAsync();
            return null;
        }

        return _current;
    }

    public async Task SignInAsync(AuthResponse response)
    {
        _current = new StoredSession(
            response.Token,
            response.ExpiresAt,
            response.UserId,
            response.Email,
            response.FullName,
            response.Role);
        _loaded = true;

        await _storage.SetAsync(StorageKey, _current);
        Changed?.Invoke();
    }

    public async Task SignOutAsync()
    {
        var wasSignedIn = _current is not null;

        _current = null;
        _loaded = true;

        try
        {
            await _storage.DeleteAsync(StorageKey);
        }
        catch (Exception ex) when (ex is InvalidOperationException or JSException)
        {
            // сховище недоступне — у пам'яті сесію вже скинуто
        }

        if (wasSignedIn)
            Changed?.Invoke();
    }
}
