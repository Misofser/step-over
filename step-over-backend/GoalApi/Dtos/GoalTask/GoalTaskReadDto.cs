namespace GoalApi.Dtos.GoalTask;

/// <summary>Represents a task belonging to a goal, returned from the API.</summary>
public class GoalTaskReadDto
{
    /// <summary>Unique identifier of the task</summary>
    public int Id { get; set; }

    /// <summary>Title of the task</summary>
    public string Title { get; set; } = null!;

    /// <summary>Indicates whether the task has been completed</summary>
    public bool IsCompleted { get; set; }
}
