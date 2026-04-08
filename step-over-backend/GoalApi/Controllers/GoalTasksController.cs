using GoalApi.Dtos.GoalTask;
using GoalApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GoalApi.Controllers;

/// <summary>
/// Manages tasks.
/// Provides endpoints for creating, retrieving, updating, and deleting tasks,
/// including updating task details and managing task completion status.
/// All endpoints require <b>authentication</b>. Deleting and creating tasks requires an <b>admin</b> role.
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

    /// <summary>
    /// Retrieves a specific task by its ID.
    /// </summary>
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
        var task = await _goalTaskService.GetTaskByIdAsync(taskId);
        return Ok(task);
    }

    /// <summary>Creates a new task for a specific goal. Admin role required</summary>
    /// <param name="goalId">The identifier of the goal to which the task will be added</param>
    /// <param name="dto">Task data required to create a new goal task</param>
    /// <response code="201">Task successfully created</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="404">Goal with the specified id was not found</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(GoalTaskReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GoalTaskReadDto>> Create(int goalId, GoalTaskCreateDto dto)
    {
        var task = await _goalTaskService.AddTaskAsync(goalId, dto);
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
        await _goalTaskService.UpdateCompletionAsync(taskId, dto);
        return NoContent();
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
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
        await _goalTaskService.UpdateTaskAsync(taskId, dto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a task. Admin role required.
    /// </summary>
    /// <param name="taskId">The ID of the task to delete</param>
    /// <response code="204">Task successfully deleted</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User does not have permission</response>
    /// <response code="404">Task not found</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("/api/tasks/{taskId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)] 
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int taskId)
    {
        await _goalTaskService.DeleteTaskAsync(taskId);
        return NoContent();
    }
}
