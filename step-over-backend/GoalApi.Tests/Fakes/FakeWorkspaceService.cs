using GoalApi.Services.Interfaces;

namespace GoalApi.Tests.Fakes;

public sealed class FakeWorkspaceService(int workspaceId = 1) : IWorkspaceService
{
    private readonly int _workspaceId = workspaceId;

    public Task<int> GetPersonalWorkspaceIdAsync(int userId)
        => Task.FromResult(_workspaceId);

    public void InitializePersonalWorkspace(User owner)
    {
    }
}
