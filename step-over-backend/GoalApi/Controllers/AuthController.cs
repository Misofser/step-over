using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GoalApi.Dtos.User;
using GoalApi.Dtos.Auth;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;

namespace GoalApi.Controllers;

/// <summary>
/// Handles user authentication and session management.
/// The login and logout endpoints are publicly accessible.
/// All other endpoints require <b>authentication</b>.
/// </summary>
[ApiController]
[Route("api/auth")]
[Authorize]
[Produces("application/json")]
public class AuthController(IAuthService authService, ICurrentUserService currentUser) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ICurrentUserService _currentUser = currentUser;

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
        var result = await _authService.LoginAsync(dto);

        Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(15)
        });

        Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromDays(7)
        });

        return Ok(result.User);
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
        var dto = new UserReadDto
        {
            Id = _currentUser.GetUserId(),
            Username = _currentUser.GetUsername(),
            Role = _currentUser.GetRole()
        };

        return Ok(dto);
    }

    /// <summary>
    /// Refreshes JWT access token using HttpOnly refresh token cookie.
    /// </summary>
    /// <returns>New access token is set in HttpOnly cookie</returns>
    /// <response code="200">Access and refresh tokens were rotated successfully</response>
    /// <response code="401">Refresh token is missing or invalid</response>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (refreshToken == null) return Unauthorized();
        var result = await _authService.RefreshAsync(refreshToken);

        Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(15)
        });

        Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromDays(7)
        });

        return Ok();
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <response code="204">Logout successful</response>
    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(Request.Cookies["refresh_token"]);
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return NoContent();
    }
}
