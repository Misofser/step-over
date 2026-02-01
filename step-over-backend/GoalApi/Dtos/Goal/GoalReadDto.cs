using GoalApi.Enums;

namespace GoalApi.Dtos.Goal;

/// <summary>
/// Represents a goal returned from the API.
/// </summary>
public class GoalReadDto
{
    /// <summary>
    /// Unique identifier of the goal.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Title of the goal.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Indicates whether the goal has been completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Type of the goal. Can be "Project" for result-based goals or "Process" for ongoing routines.
    /// </summary>
    public GoalType Type { get; set; }
}
