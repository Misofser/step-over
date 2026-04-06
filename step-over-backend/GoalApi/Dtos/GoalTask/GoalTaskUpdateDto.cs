using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.GoalTask;

/// <summary>
/// Data used to update an existing task.
/// </summary>
public class GoalTaskUpdateDto
{
    /// <summary>
    /// New title of the task. Optional, but cannot be empty if provided.
    /// </summary>
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    public string? Title { get; set; } = null!;
}
