using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos;

public class GoalCreateDto
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    public string Title { get; set; } = "";
}
