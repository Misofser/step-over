using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GoalApi.Dtos.User;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;

namespace GoalApi.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController(IAuthService authService, IJwtService jwt) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IJwtService _jwt = jwt;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var readDto = await _authService.LoginAsync(dto);
        var token = _jwt.GenerateToken(userId: readDto.Id, username: readDto.Username, role: readDto.Role);

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // TODO: set true in production
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return Ok(readDto);
    }

    [HttpGet("me")]
    public ActionResult<UserReadDto> Me()
    {
        var dto = _authService.GetCurrentUser();
        return Ok(dto);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok();
    }
}
