using GoalApi.Dtos.Thought;
using GoalApi.Enums;
using GoalApi.Services;

namespace GoalApi.Tests.Services;

public class ThoughtServiceTests
{
    [Fact]
    public async Task CreateThoughtAsync_CreatesInboxThoughtForCurrentUserInPersonalWorkspace()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var anotherUser = new User { Username = "Another User", PasswordHash = "anothertesthash" };
        var personalWorkspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var anotherWorkspace = new Workspace
        {
            Name = "Another Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = anotherUser, Role = WorkspaceRole.Owner } }
        };
        db.Workspaces.AddRange(personalWorkspace, anotherWorkspace);
        await db.SaveChangesAsync();

        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));
        var dto = new ThoughtCreateDto { Content = "Test thought one." };

        // Act
        var result = await service.CreateThoughtAsync(user.Id, dto);

        // Assert
        Assert.Equal("Test thought one.", result.Content);
        Assert.Equal(ThoughtStatus.Inbox, result.Status);

        var thought = await db.Thoughts.SingleAsync();
        Assert.Equal(user.Id, thought.UserId);
        Assert.Equal(personalWorkspace.Id, thought.WorkspaceId);
        Assert.Equal(ThoughtStatus.Inbox, thought.Status);
        Assert.NotEqual(anotherUser.Id, thought.UserId);
        Assert.NotEqual(anotherWorkspace.Id, thought.WorkspaceId);
    }
}
