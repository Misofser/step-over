using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.GoalTask;

/// <summary>Data required to create a new goal task.</summary>
public class GoalTaskCreateDto
{
    /// <summary>Title of the task</summary>
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = null!;
}
