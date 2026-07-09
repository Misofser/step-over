using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Enums;
using GoalApi.Services.Interfaces;

namespace GoalApi.Services;

public class WorkspaceService(AppDbContext db) : IWorkspaceService
{
    private readonly AppDbContext _db = db;

    public void InitializePersonalWorkspace(User owner)
    {
        var workspace = new Workspace
        {
            Name = $"{owner.Username}'s Workspace",
            Type = WorkspaceType.Personal,
            Members =
            {
                new WorkspaceMember
                {
                    User = owner,
                    Role = WorkspaceRole.Owner
                }
            }
        };

        _db.Workspaces.Add(workspace);
    }
}
