using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GoalApi.Data;
using GoalApi.Dtos.User;
using GoalApi.Models;
using GoalApi.Services;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null) return Unauthorized();

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized();

        var token = _jwt.GenerateToken(user);

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // true in production (https)
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return Ok(new { role = user.Role });
    }

    [HttpGet("me")]
    public ActionResult<MeDto> Me()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (username == null || role == null)
        {
            return Unauthorized();
        }

        var dto = new MeDto
        {
            Username = username,
            Role = role,
        };

        return Ok(dto);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");

        return Ok();
    }
}
