using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.Goal;

/// <summary>
/// Data used to update an existing goal.
/// </summary>
public class GoalUpdateDto
{
    /// <summary>
    /// New title of the goal. Optional, but cannot be empty if provided.
    /// </summary>
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    public string? Title { get; set; } = null!;
    /// <summary>
    /// Indicates whether the goal is completed. Optional. If provided, updates the completion status of the goal.
    /// </summary>
    public bool? IsCompleted { get; set; }
}
