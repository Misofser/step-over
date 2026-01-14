using System.ComponentModel.DataAnnotations;

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
}
