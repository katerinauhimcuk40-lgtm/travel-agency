using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TravelAgency.Api.Auth;
using TravelAgency.Api.Dtos;
using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;

namespace TravelAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ITokenService _tokens;

    public AuthController(IAuthService auth, ITokenService tokens)
    {
        _auth = auth;
        _tokens = tokens;
    }

    /// <summary>UC-01: зареєструвати нового клієнта. Роль завжди Client.</summary>
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var user = await _auth.RegisterAsync(request.Email, request.Password, request.FullName, request.Phone);
        return Ok(BuildResponse(user));
    }

    /// <summary>UC-02: увійти за email і паролем.</summary>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _auth.LoginAsync(request.Email, request.Password);
        return Ok(BuildResponse(user));
    }

    private AuthResponse BuildResponse(User user)
    {
        var (token, expiresAt) = _tokens.CreateToken(user);
        return new AuthResponse(token, expiresAt, user.Id, user.Email, user.FullName, user.Role);
    }
}
