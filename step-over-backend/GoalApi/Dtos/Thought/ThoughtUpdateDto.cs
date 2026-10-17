using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.Thought;

/// <summary>Data required to update an existing thought.</summary>
public class ThoughtUpdateDto
{
    /// <summary>The updated content of the thought.</summary>
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = null!;
}
