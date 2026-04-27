using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.Habit;

/// <summary>Data for toggling the completion status of an existing habit for a specific date.</summary>
public class HabitToggleDto
{
    /// <summary>The date for which the habit completion status should be toggled</summary>
    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }
}
