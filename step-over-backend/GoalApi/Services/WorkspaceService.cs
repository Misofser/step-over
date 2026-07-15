using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Enums;
using GoalApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public Task<int> GetPersonalWorkspaceIdAsync(int userId)
    {
        return _db.WorkspaceMembers
            .Where(wm => wm.UserId == userId && wm.Workspace.Type == WorkspaceType.Personal)
            .Select(wm => wm.WorkspaceId)
            .SingleAsync();
    }
}
