using GoalApi.Enums;

namespace GoalApi.Models;

public class Thought : IHasTimestamps
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public ThoughtStatus Status { get; set; } = ThoughtStatus.Inbox;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
