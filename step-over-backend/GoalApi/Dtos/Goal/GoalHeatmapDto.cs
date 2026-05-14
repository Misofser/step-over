namespace GoalApi.Dtos.Goal;

/// <summary>
/// Represents a single day in a goal heatmap, showing how many habits were completed on that date
/// compared to the total number of habits in the goal.
/// </summary>
public class GoalHeatmapDto
{
    /// <summary>Date of activity.</summary>
    public DateTime Date { get; set; }

    /// <summary>Number of completed habits for this day.</summary>
    public int CompletedHabits { get; set; }

    /// <summary>Total habits in the goal.</summary>
    public int TotalHabits { get; set; }
}
