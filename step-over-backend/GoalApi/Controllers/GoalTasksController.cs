using GoalApi.Dtos.GoalTask;
using GoalApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GoalApi.Controllers;

/// <summary>
/// Manages goal tasks.
/// Provides endpoints to create, update, delete, and retrieve tasks associated with a specific goal.
/// All endpoints require <b>authentication</b>.
/// </summary>
[ApiController]
[Route("api/goals/{goalId}/tasks")]
[Authorize]
[Produces("application/json")]
public class GoalTasksController(IGoalTaskService goalTaskService) : ControllerBase
{
    private readonly IGoalTaskService _goalTaskService = goalTaskService;

    /// <summary>Retrieves all tasks associated with a specific goal.</summary>
    /// <param name="goalId">The identifier of the goal to which the task will be added</param>
    /// <returns>A list of goal tasks.</returns>
    /// <response code="200">Returns the list of goals</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Goal with the specified id was not found</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GoalTaskReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GoalTaskReadDto>>> GetTasks(int goalId)
    {
        var tasks = await _goalTaskService.GetTasksByGoalAsync(goalId);
        return Ok(tasks);
    }

    /// <summary>Creates a new task for a specific goal. Admin role required</summary>
    /// <param name="goalId">The identifier of the goal to which the task will be added</param>
    /// <param name="dto">Task data required to create a new goal task</param>
    /// <returns>Returns <c>200 OK</c> if the task was successfully created</returns>
    /// <response code="200">Task was created successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="404">Goal with the specified id was not found</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int goalId, GoalTaskCreateDto dto)
    {
        await _goalTaskService.AddTaskAsync(goalId, dto);
        return Ok();
    }
}
