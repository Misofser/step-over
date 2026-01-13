using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.Goal;

public class GoalCreateDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = null!;
}
