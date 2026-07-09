using GoalApi.Enums;

namespace GoalApi.Models;

public class WorkspaceMember : IHasTimestamps
{
    public int Id { get; set; }

    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public WorkspaceRole Role { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
