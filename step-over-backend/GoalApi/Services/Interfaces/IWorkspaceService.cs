using GoalApi.Models;

namespace GoalApi.Services.Interfaces;

public interface IWorkspaceService
{
    void InitializePersonalWorkspace(User owner);
}
