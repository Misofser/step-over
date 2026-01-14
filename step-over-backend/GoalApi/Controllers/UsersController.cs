using GoalApi.Exceptions;
using GoalApi.Dtos.User;
using GoalApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GoalApi.Controllers;

/// <summary>
/// Manages users.
/// Provides endpoints to create, update, delete, and retrieve users.
/// Access to all endpoints requires <b>authentication</b> and the <b>Admin</b> role.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Retrieves all users. Admin role required.
    /// </summary>
    /// <returns>A list of users.</returns>
    /// <response code="200">Returns the list of users</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserReadDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Retrieves a user by his ID. Admin role required.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The requested user.</returns>
    /// <response code="200">Returns the requested user</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserReadDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    /// <summary>
    /// Creates a new user. Admin role required.
    /// </summary>
    /// <param name="dto">Data required to create a user</param>
    /// <returns>The created user</returns>
    /// <response code="201">User successfully created</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="409">User with this username already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserReadDto>> Create(UserCreateDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Updates an existing user. Admin role required.
    /// </summary>
    /// <param name="id">The ID of the user to update</param>
    /// <param name="updatedUser">Data to update the user</param>
    /// <response code="204">User successfully updated</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="404">User not found</response>
    /// <response code="409">User with this username already exists</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, UserUpdateDto updatedUser)
    {
        await _userService.UpdateUserAsync(id, updatedUser);
        return NoContent();
    }

    /// <summary>
    /// Deletes a user. Admin role required.
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <response code="204">User successfully deleted</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="404">User not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)] 
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
