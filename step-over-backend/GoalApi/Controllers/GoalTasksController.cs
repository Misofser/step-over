using GoalApi.Dtos.GoalTask;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GoalApi.Controllers;

/// <summary>
/// Manages tasks.
/// Provides endpoints for creating, retrieving, updating, and deleting tasks,
/// including updating task details and managing task completion status.
/// All endpoints require <b>authentication</b>.
/// </summary>
[ApiController]
[Route("api/goals/{goalId}/tasks")]
[Authorize]
[Produces("application/json")]
public class GoalTasksController(IGoalTaskService goalTaskService, ICurrentUserService currentUser) : ControllerBase
{
    private readonly IGoalTaskService _goalTaskService = goalTaskService;
    private readonly ICurrentUserService _currentUser = currentUser;

    /// <summary>Retrieves all tasks associated with a specific goal</summary>
    /// <param name="goalId">The ID of the goal to retrieve tasks for.</param>
    /// <returns>A list of goal tasks.</returns>
    /// <response code="200">Returns the list of tasks</response>
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
        var userId = _currentUser.GetUserId();
        var tasks = await _goalTaskService.GetTasksByGoalAsync(userId, goalId);
        return Ok(tasks);
    }

    /// <summary>Retrieves a specific task by its ID</summary>
    /// <param name="taskId">The ID of the task to retrieve</param>
    /// <returns>The requested task.</returns>
    /// <response code="200">Returns the requested task</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Task not found</response>
    [HttpGet("/api/tasks/{taskId}")]
    [ProducesResponseType(typeof(GoalTaskReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GoalTaskReadDto>> GetTask(int taskId)
    {
        var userId = _currentUser.GetUserId();
        var task = await _goalTaskService.GetTaskByIdAsync(userId, taskId);
        return Ok(task);
    }

    /// <summary>Creates a new task for a specific goal</summary>
    /// <param name="goalId">The identifier of the goal to which the task will be added</param>
    /// <param name="dto">Task data required to create a new goal task</param>
    /// <response code="201">Task successfully created</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Goal with the specified id was not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(GoalTaskReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GoalTaskReadDto>> Create(int goalId, GoalTaskCreateDto dto)
    {
        var userId = _currentUser.GetUserId();
        var task = await _goalTaskService.AddTaskAsync(userId, goalId, dto);
        return CreatedAtAction(nameof(GetTask), new { taskId = task.Id }, task);
    }

    /// <summary>Updates the completion status of a specific task</summary>
    /// <param name="taskId">The ID of the task to update</param>
    /// <param name="dto">The data containing the new completion status</param>
    /// <response code="204">The task completion status was updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Task with the specified id was not found</response>
    [HttpPatch("/api/tasks/{taskId}/completion")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)] 
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompletion(int taskId, [FromBody] GoalTaskUpdateCompletionDto dto)
    {
        var userId = _currentUser.GetUserId();
        await _goalTaskService.UpdateCompletionAsync(userId, taskId, dto);
        return NoContent();
    }

    /// <summary>Updates an existing task</summary>
    /// <param name="taskId">The ID of the task to update</param>
    /// <param name="dto">Data for updating the task</param>
    /// <response code="204">Task successfully updated</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Task not found</response>
    [HttpPatch("/api/tasks/{taskId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int taskId, GoalTaskUpdateDto dto)
    {
        var userId = _currentUser.GetUserId();
        await _goalTaskService.UpdateTaskAsync(userId, taskId, dto);
        return NoContent();
    }

    /// <summary>Deletes a task</summary>
    /// <param name="taskId">The ID of the task to delete</param>
    /// <response code="204">Task successfully deleted</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="404">Task not found</response>
    [HttpDelete("/api/tasks/{taskId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)] 
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int taskId)
    {
        var userId = _currentUser.GetUserId();
        await _goalTaskService.DeleteTaskAsync(userId, taskId);
        return NoContent();
    }
}
