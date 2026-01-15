using GoalApi.Dtos.Goal;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GoalApi.Controllers;

/// <summary>
/// Manages goals.
/// Provides endpoints to create, update, delete, and retrieve goals.
/// All endpoints require <b>authentication</b>.
/// </summary>
[ApiController]
[Route("api/goals")]
[Authorize]
[Produces("application/json")]
public class GoalsController(IGoalService goalService, ICurrentUserService currentUser) : ControllerBase
{
    private readonly IGoalService _goalService = goalService;
    private readonly ICurrentUserService _currentUser = currentUser;

    /// <summary>
    /// Retrieves all goals.
    /// </summary>
    /// <returns>A list of goals.</returns>
    /// <response code="200">Returns the list of goals</response>
    /// <response code="401">User is unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GoalReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GoalReadDto>>> GetGoals()
    {
        var goals = await _goalService.GetAllGoalsAsync();
        return Ok(goals);
    }

    /// <summary>
    /// Retrieves a specific goal by its ID.
    /// </summary>
    /// <param name="id">The ID of the goal to retrieve</param>
    /// <returns>The requested goal.</returns>
    /// <response code="200">Returns the requested goal</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Goal not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GoalReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GoalReadDto>> GetGoal(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        return Ok(goal);
    }

    /// <summary>
    /// Creates a new goal. Admin role required.
    /// </summary>
    /// <param name="dto">Data required to create the goal</param>
    /// <returns>The created goal</returns>
    /// <response code="201">Goal successfully created</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(GoalReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GoalReadDto>> Create(GoalCreateDto dto)
    {
        var userId = _currentUser.GetUserId();
        var goal = await _goalService.CreateGoalAsync(userId, dto);
        return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
    }

    /// <summary>
    /// Updates an existing goal.
    /// </summary>
    /// <param name="id">The ID of the goal to update</param>
    /// <param name="dto">Data for updating the goal</param>
    /// <response code="204">Goal successfully updated</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Goal not found</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, GoalUpdateDto dto)
    {
        await _goalService.UpdateGoalAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a goal. Admin role required.
    /// </summary>
    /// <param name="id">The ID of the goal to delete</param>
    /// <response code="204">Goal successfully deleted</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="404">Goal not found</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)] 
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _goalService.DeleteGoalAsync(id);
        return NoContent();
    }

}
