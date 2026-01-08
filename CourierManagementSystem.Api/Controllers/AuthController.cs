using CourierManagementSystem.Api.Models.DTOs.Requests;
using CourierManagementSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CourierManagementSystem.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

[HttpPost("login")]
public IActionResult Login(LoginRequest request)
{
    var user = AuthenticateUser(request);
    var token = GenerateJwtToken(user);
    return Ok(new { Token = token });
}
private User AuthenticateUser(LoginRequest request)
{
    var user = _context.Users.FirstOrDefault(x => x.Email == request.Email);

    if (user == null || !BCrypt.Verify(request.Password, user.PasswordHash))
        throw new UnauthorizedAccessException();

    return user;
}

private string GenerateJwtToken(User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.Id.ToString())
        }),
        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
    };

    return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
}

    [HttpGet("debug")]
    [AllowAnonymous]
    public IActionResult Debug()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var login = User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            userId,
            login,
            role,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }
}
