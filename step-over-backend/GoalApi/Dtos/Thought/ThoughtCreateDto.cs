using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.Thought;

/// <summary>Data required to create a new thought.</summary>
public class ThoughtCreateDto
{
    /// <summary>The content of the thought.</summary>
    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = null!;
}
