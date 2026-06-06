namespace GoalApi.Dtos.Habit;

/// <summary>Represents the completion status of a habit for a specific date.</summary>
public class HabitCompletionStatusDto
{
    /// <summary>The date for which the completion status is returned</summary>
    public DateTime Date { get; set; }

    /// <summary>Indicates whether the habit was completed on the specified date</summary>
    public bool IsCompleted { get; set; }
}
