using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GoalApi.Dtos.User;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;

namespace GoalApi.Controllers;

/// <summary>
/// Handles user authentication and session management.
/// The login endpoint is publicly accessible.
/// All other endpoints require <b>authentication</b>.
/// </summary>
[ApiController]
[Route("api/auth")]
[Authorize]
[Produces("application/json")]
public class AuthController(IAuthService authService, IJwtService jwt) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IJwtService _jwt = jwt;

    /// <summary>
    /// Logs in a user and returns user info.
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>User information</returns>
    /// <response code="200">Login successful, returns user info</response>
    /// <response code="401">Invalid username or password</response>
    /// <response code="400">Invalid request data</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Returns information about the currently authenticated user.
    /// </summary>
    /// <returns>Current user information</returns>
    /// <response code="200">Returns the current user's info</response>
    /// <response code="401">User is unauthorized</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public ActionResult<UserReadDto> Me()
    {
        var dto = _authService.GetCurrentUser();
        return Ok(dto);
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <response code="200">Logout successful</response>
    /// <response code="401">User is unauthorized</response>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok();
    }
}
