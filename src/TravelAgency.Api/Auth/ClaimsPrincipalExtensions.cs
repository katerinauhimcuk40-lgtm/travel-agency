using System.Security.Claims;

namespace TravelAgency.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Id поточного користувача з claim "sub".</summary>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirst("sub")?.Value;

        return int.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedAccessException("User id claim is missing.");
    }

    /// <summary>Менеджер або адміністратор.</summary>
    public static bool IsStaff(this ClaimsPrincipal principal) =>
        principal.IsInRole("Manager") || principal.IsInRole("Admin");
}
