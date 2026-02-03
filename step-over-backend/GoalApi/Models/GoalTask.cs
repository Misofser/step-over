namespace GoalApi.Models;

public class GoalTask : IHasTimestamps
{
    public int Id { get; set; }
    public int GoalId { get; set; }

    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Goal Goal { get; set; } = null!;
}
