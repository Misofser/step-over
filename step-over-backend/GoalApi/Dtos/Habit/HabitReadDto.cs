using GoalApi.Enums;

namespace GoalApi.Dtos.Habit;

/// <summary>Represents a habit belonging to a goal, returned from the API.</summary>
public class HabitReadDto
{
    /// <summary>Unique identifier of the habit</summary>
    public int Id { get; set; }

    /// <summary>Title of the habit</summary>
    public string Title { get; set; } = null!;

    /// <summary>Specifies how often the habit should be performed</summary>
    public HabitFrequency Frequency { get; set; }

    /// <summary>Indicates whether the habit has been completed today</summary>
    public bool IsCompletedToday { get; set; }
}
