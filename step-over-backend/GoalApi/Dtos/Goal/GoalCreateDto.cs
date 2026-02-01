using System.ComponentModel.DataAnnotations;
using GoalApi.Enums;

namespace GoalApi.Dtos.Goal;

/// <summary>
/// Data required to create a new goal.
/// </summary>
public class GoalCreateDto
{
    /// <summary>
    /// Title of the goal.
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Type of the goal. Can be "Project" for result-based goals or "Process" for ongoing routines.
    /// </summary>
    [Required(ErrorMessage = "Goal type is required")]
    public GoalType Type { get; set; }
}
