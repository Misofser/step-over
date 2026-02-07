using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.GoalTask;

/// <summary>Data to update the completion status of an existing task.</summary>
public class GoalTaskUpdateCompletionDto
{
    /// <summary>Indicates whether the task has been completed</summary>
    [Required(ErrorMessage = "Completion is required")]
    public bool? IsCompleted { get; set; }
}
