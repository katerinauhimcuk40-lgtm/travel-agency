using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using TravelAgency.Core.Entities;

namespace TravelAgency.Api.Auth;

public class JwtTokenService : ITokenService
{
    /// <summary>Короткі імена claim-ів: у Program.cs вимкнено MapInboundClaims, тож ці ж імена читаються на сервері.</summary>
    public const string SubjectClaim = "sub";
    public const string RoleClaim = "role";
    public const string NameClaim = "name";

    private readonly SigningCredentials _credentials;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiresMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Configuration value 'Jwt:Key' is missing.");

        _credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        _issuer = configuration["Jwt:Issuer"] ?? "TravelAgency";
        _audience = configuration["Jwt:Audience"] ?? "TravelAgency.Clients";
        _expiresMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var minutes) && minutes > 0 ? minutes : 120;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_expiresMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Expires = expiresAt,
            SigningCredentials = _credentials,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(SubjectClaim, user.Id.ToString()),
                new Claim(NameClaim, user.FullName),
                new Claim(RoleClaim, user.Role.ToString())
            })
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return (token, expiresAt);
    }
}
