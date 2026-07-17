using GoalApi.Dtos.Habit;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GoalApi.Controllers;

/// <summary>
/// Manages habits.
/// Provides authenticated endpoints for retrieving, creating, tracking completion,
/// and deleting habits.
/// </summary>
[ApiController]
[Route("api/goals/{goalId}/habits")]
[Authorize]
[Produces("application/json")]
public class HabitsController(IHabitService habitService, ICurrentUserService currentUser) : ControllerBase
{
    private readonly IHabitService _habitService = habitService;
    private readonly ICurrentUserService _currentUser = currentUser;

    /// <summary>Retrieves all habits associated with a specific goal</summary>
    /// <param name="goalId">The ID of the goal to retrieve habits for.</param>
    /// <returns>A list of goal habits.</returns>
    /// <response code="200">Returns the list of habits</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Goal with the specified id was not found</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<HabitReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HabitReadDto>>> GetHabits(int goalId)
    {
        var userId = _currentUser.GetUserId();
        var habits = await _habitService.GetHabitsByGoalAsync(userId, goalId);
        return Ok(habits);
    }

    /// <summary>Retrieves a specific habit by its ID</summary>
    /// <param name="habitId">The ID of the habit to retrieve</param>
    /// <returns>The requested habit.</returns>
    /// <response code="200">Returns the requested habit</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Habit not found</response>
    [HttpGet("/api/habits/{habitId}")]
    [ProducesResponseType(typeof(HabitReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitReadDto>> GetHabit(int habitId)
    {
        var userId = _currentUser.GetUserId();
        var habit = await _habitService.GetHabitByIdAsync(userId, habitId);
        return Ok(habit);
    }

    /// <summary>Returns completion status of a specific habit for a given date</summary>
    /// <param name="habitId">The ID of the habit</param>
    /// <param name="date">Date to check completion status for</param>
    /// <returns>Completion status for the specified habit and date</returns>
    /// <response code="200">Returns the сompletion status for the specified habit and date</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Habit not found</response>
    [HttpGet("/api/habits/{habitId}/completion")]
    [ProducesResponseType(typeof(HabitCompletionStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitCompletionStatusDto>> GetCompletionStatus(int habitId, [FromQuery] DateTime date)
    {
        var userId = _currentUser.GetUserId();
        var result = await _habitService.GetCompletionStatusAsync(userId, habitId, date);
        return Ok(result);
    }

    /// <summary>Creates a new habit for a specific goal</summary>
    /// <param name="goalId">The identifier of the goal to which the habit will be added</param>
    /// <param name="dto">Habit data required to create a new habit</param>
    /// <response code="201">Habit successfully created</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Goal with the specified id was not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(HabitReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitReadDto>> Create(int goalId, HabitCreateDto dto)
    {
        var userId = _currentUser.GetUserId();
        var habit = await _habitService.AddHabitAsync(userId, goalId, dto);
        return CreatedAtAction(nameof(GetHabit), new { habitId = habit.Id }, habit);
    }

    /// <summary>Toggles completion status of a habit for a specific date</summary>
    /// <param name="habitId">The ID of the habit</param>
    /// <param name="dto">Date for which to toggle completion</param>
    /// <response code="204">Completion successfully toggled</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Habit not found</response>
    [HttpPost("/api/habits/{habitId}/toggle")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Toggle(int habitId, HabitToggleDto dto)
    {
        var userId = _currentUser.GetUserId();
        await _habitService.ToggleCompletion(userId, habitId, dto.Date);
        return NoContent();
    }

    /// <summary>Deletes a habit</summary>
    /// <param name="habitId">The ID of the habit to delete</param>
    /// <response code="204">Habit successfully deleted</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Habit not found</response>
    [HttpDelete("/api/habits/{habitId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)] 
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int habitId)
    {
        var userId = _currentUser.GetUserId();
        await _habitService.DeleteHabitAsync(userId, habitId);
        return NoContent();
    }
}
