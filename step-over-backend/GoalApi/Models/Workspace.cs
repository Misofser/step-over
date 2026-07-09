using GoalApi.Enums;

namespace GoalApi.Models;

public class Workspace : IHasTimestamps
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public WorkspaceType Type { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<WorkspaceMember> Members { get; set; } = new();
}
