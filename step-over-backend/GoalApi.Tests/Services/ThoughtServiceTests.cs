using GoalApi.Dtos.Thought;
using GoalApi.Enums;
using GoalApi.Services;

namespace GoalApi.Tests.Services;

public class ThoughtServiceTests
{
    [Fact]
    public async Task GetAllThoughtsAsync_ReturnsAllThoughtsForCurrentUserInPersonalWorkspace()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var personalWorkspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        db.Thoughts.AddRange(
            new Thought
            {
                Content = "First thought",
                User = user,
                Workspace = personalWorkspace
            },
            new Thought
            {
                Content = "Second thought",
                User = user,
                Workspace = personalWorkspace
            });
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));

        // Act
        var result = await service.GetAllThoughtsAsync(user.Id);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, thought => thought.Content == "First thought");
        Assert.Contains(result, thought => thought.Content == "Second thought");
    }

    [Fact]
    public async Task GetAllThoughtsAsync_DoesNotReturnThoughtsBelongingToAnotherUser()
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
        db.Thoughts.AddRange(
            new Thought
            {
                Content = "Current user thought",
                User = user,
                Workspace = personalWorkspace
            },
            new Thought
            {
                Content = "Another user thought",
                User = anotherUser,
                Workspace = anotherWorkspace
            });
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));

        // Act
        var result = await service.GetAllThoughtsAsync(user.Id);

        // Assert
        var contents = result.Select(thought => thought.Content).ToList();
        Assert.Contains("Current user thought", contents);
        Assert.DoesNotContain("Another user thought", contents);
    }

    [Fact]
    public async Task GetAllThoughtsAsync_ReturnsEmptyCollection_WhenCurrentUserHasNoThoughts()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var personalWorkspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        db.Workspaces.Add(personalWorkspace);
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));

        // Act
        var result = await service.GetAllThoughtsAsync(user.Id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetThoughtByIdAsync_ReturnsThought()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var personalWorkspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var thought = new Thought
        {
            Content = "A thought to retrieve",
            User = user,
            Workspace = personalWorkspace
        };
        db.Thoughts.Add(thought);
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));

        // Act
        var result = await service.GetThoughtByIdAsync(user.Id, thought.Id);

        // Assert
        Assert.Equal(thought.Id, result.Id);
        Assert.Equal("A thought to retrieve", result.Content);
        Assert.Equal(ThoughtStatus.Inbox, result.Status);
    }

    [Fact]
    public async Task GetThoughtByIdAsync_ThrowsNotFoundException_WhenThoughtDoesNotExist()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var service = new ThoughtService(db, new FakeWorkspaceService());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetThoughtByIdAsync(userId: 1, thoughtId: 1)
        );
    }

    [Fact]
    public async Task GetThoughtByIdAsync_ThrowsNotFoundException_WhenThoughtIsInAnotherWorkspace()
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
        var thought = new Thought
        {
            Content = "Another user's thought",
            User = anotherUser,
            Workspace = anotherWorkspace
        };
        db.Workspaces.Add(personalWorkspace);
        db.Thoughts.Add(thought);
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetThoughtByIdAsync(user.Id, thought.Id)
        );
    }

    [Fact]
    public async Task GetThoughtByIdAsync_ReturnsArchivedThought()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var personalWorkspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var thought = new Thought
        {
            Content = "Archived thought",
            Status = ThoughtStatus.Archived,
            User = user,
            Workspace = personalWorkspace
        };
        db.Thoughts.Add(thought);
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));

        // Act
        var result = await service.GetThoughtByIdAsync(user.Id, thought.Id);

        // Assert
        Assert.Equal(thought.Id, result.Id);
        Assert.Equal("Archived thought", result.Content);
        Assert.Equal(ThoughtStatus.Archived, result.Status);
    }

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

    [Fact]
    public async Task UpdateThoughtAsync_UpdatesContent()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var personalWorkspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var thought = new Thought
        {
            Content = "Original thought",
            Status = ThoughtStatus.Archived,
            User = user,
            Workspace = personalWorkspace
        };
        db.Thoughts.Add(thought);
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));
        var dto = new ThoughtUpdateDto { Content = "  Updated thought  " };

        // Act
        await service.UpdateThoughtAsync(user.Id, thought.Id, dto);

        // Assert
        var updatedThought = await db.Thoughts.FindAsync(thought.Id);
        Assert.NotNull(updatedThought);
        Assert.Equal("Updated thought", updatedThought!.Content);
        Assert.Equal(ThoughtStatus.Archived, updatedThought.Status);
    }

    [Fact]
    public async Task UpdateThoughtAsync_ThrowsNotFoundException_WhenThoughtDoesNotExist()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var service = new ThoughtService(db, new FakeWorkspaceService());
        var dto = new ThoughtUpdateDto { Content = "Updated thought" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateThoughtAsync(userId: 1, thoughtId: 1, dto)
        );
    }

    [Fact]
    public async Task UpdateThoughtAsync_ThrowsNotFoundException_WhenThoughtIsInAnotherWorkspace()
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
        var thought = new Thought
        {
            Content = "Another user's thought",
            User = anotherUser,
            Workspace = anotherWorkspace
        };
        db.Workspaces.Add(personalWorkspace);
        db.Thoughts.Add(thought);
        await db.SaveChangesAsync();
        var service = new ThoughtService(db, new FakeWorkspaceService(personalWorkspace.Id));
        var dto = new ThoughtUpdateDto { Content = "Updated thought" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateThoughtAsync(user.Id, thought.Id, dto)
        );

        var thoughtInDb = await db.Thoughts.FindAsync(thought.Id);
        Assert.NotNull(thoughtInDb);
        Assert.Equal("Another user's thought", thoughtInDb!.Content);
    }
}
