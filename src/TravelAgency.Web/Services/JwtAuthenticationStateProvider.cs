using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

namespace TravelAgency.Web.Services;

/// <summary>Будує ClaimsPrincipal із поточної сесії, щоб працювали AuthorizeView та атрибут [Authorize] на сторінках.</summary>
public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly AuthSession _session;

    public JwtAuthenticationStateProvider(AuthSession session)
    {
        _session = session;
        _session.Changed += OnSessionChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await _session.GetAsync();
        if (session is null)
            return Anonymous;

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                new Claim(ClaimTypes.Name, session.FullName),
                new Claim(ClaimTypes.Email, session.Email),
                new Claim(ClaimTypes.Role, session.Role.ToString())
            },
            authenticationType: "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private void OnSessionChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public void Dispose() => _session.Changed -= OnSessionChanged;
}
