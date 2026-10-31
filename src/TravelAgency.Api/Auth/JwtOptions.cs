namespace TravelAgency.Api.Auth;

/// <summary>Налаштування JWT (секція "Jwt" у конфігурації). Ключ у робочому середовищі — змінна Jwt__Key.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TravelAgency";
    public string Audience { get; set; } = "TravelAgency.Clients";
    public int ExpiresMinutes { get; set; } = 120;
}
