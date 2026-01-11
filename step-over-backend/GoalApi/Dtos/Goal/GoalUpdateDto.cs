using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.Goal;

public class GoalUpdateDto
{
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    public string? Title { get; set; } = null!;
    public bool? IsCompleted { get; set; }
}
