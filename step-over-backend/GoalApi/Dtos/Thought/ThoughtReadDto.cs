using GoalApi.Enums;

namespace GoalApi.Dtos.Thought;

/// <summary>Represents a thought returned from the API.</summary>
public class ThoughtReadDto
{
    /// <summary>Unique identifier of the thought.</summary>
    public int Id { get; set; }

    /// <summary>The content of the thought.</summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// The current status of the thought, indicating whether it is still waiting
    /// to be reviewed by the user or has already been archived.
    /// </summary>
    public ThoughtStatus Status { get; set; }
}
