using GoalApi.Services;
using GoalApi.Enums;

namespace GoalApi.Tests.Services;

public class WorkspaceServiceTests
{
    [Fact]
    public async Task GetPersonalWorkspaceIdAsync_ReturnsUsersWorkspaceId()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var anotherUser = new User { Username = "Another User", PasswordHash = "anothertesthash" };
        var userWorkspace = new Workspace
        {
            Name = "User Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var anotherWorkspace = new Workspace
        {
            Name = "Another Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = anotherUser, Role = WorkspaceRole.Owner } }
        };
        db.Workspaces.AddRange(userWorkspace, anotherWorkspace);
        await db.SaveChangesAsync();
        var service = new WorkspaceService(db);

        // Act
        var result = await service.GetPersonalWorkspaceIdAsync(user.Id);

        // Assert
        Assert.Equal(userWorkspace.Id, result);
    }

    [Fact]
    public async Task InitializePersonalWorkspace_CreatesPersonalWorkspaceWithOwner()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "TestUser", PasswordHash = "testhash" };
        var service = new WorkspaceService(db);

        // Act
        service.InitializePersonalWorkspace(user);
        await db.SaveChangesAsync();

        // Assert
        var workspace = await db.Workspaces.Include(w => w.Members).SingleAsync();
        var member = Assert.Single(workspace.Members);
        Assert.Multiple(() =>
        {
            Assert.Equal("TestUser's Workspace", workspace.Name);
            Assert.Equal(WorkspaceType.Personal, workspace.Type);
            Assert.Equal(user.Id, member.UserId);
            Assert.Equal(WorkspaceRole.Owner, member.Role);
        });
    }
}
