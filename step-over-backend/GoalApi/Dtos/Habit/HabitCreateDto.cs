using System.ComponentModel.DataAnnotations;
using GoalApi.Enums;

namespace GoalApi.Dtos.Habit;

/// <summary>Data required to create a new habit.</summary>
public class HabitCreateDto
{
    /// <summary>Title of the habit</summary>
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = null!;

    /// <summary>Specifies how often the habit should be performed</summary>
    [Required]
    public HabitFrequency Frequency { get; set; }
}
