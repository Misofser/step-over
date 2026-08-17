using GoalApi.Dtos.Goal;
using GoalApi.Services;
using GoalApi.Enums;

namespace GoalApi.Tests.Services;

public class GoalServiceTests
{
    [Fact]
    public async Task GetAllGoalsAsync_ReturnsAllGoals()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        db.Goals.AddRange(
            new Goal
            {
                Title = "Goal 1",
                IsCompleted = false,
                User = user,
                Type = GoalType.Process,
                Workspace = workspace
            },
            new Goal
            {
                Title = "Goal 2",
                IsCompleted = true,
                User = user,
                Type = GoalType.Project,
                Workspace = workspace
            }
        );
        await db.SaveChangesAsync();

        var service = new GoalService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetAllGoalsAsync(user.Id);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Title == "Goal 1" && !g.IsCompleted && g.Type == GoalType.Process);
        Assert.Contains(result, g => g.Title == "Goal 2" && g.IsCompleted  && g.Type == GoalType.Project);
    }

    [Fact]
    public async Task GetAllGoalsAsync_ShouldReturnOnlyGoalsFromUserWorkspace()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
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
        db.Goals.AddRange(
            new Goal
            {
                Title = "User Goal",
                IsCompleted = false,
                User = user,
                Type = GoalType.Process,
                Workspace = userWorkspace
            },
            new Goal
            {
                Title = "Another Goal",
                User = anotherUser,
                IsCompleted = false,
                Type = GoalType.Process,
                Workspace = anotherWorkspace
            });
        await db.SaveChangesAsync();
        var service = new GoalService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act
        var result = await service.GetAllGoalsAsync(user.Id);

        // Assert
        var titles = result.Select(g => g.Title).ToList();
        Assert.Contains("User Goal", titles);
        Assert.DoesNotContain("Another Goal", titles);
    }

    [Fact]
    public async Task GetGoalByIdAsync_ReturnsGoal_WhenExists()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var goal = new Goal
        {
            Title = "First goal",
            IsCompleted = false,
            User = user,
            Type = GoalType.Process,
            Workspace = workspace
        };
        var goal2 = new Goal
        {
            Title = "Second goal",
            IsCompleted = true,
            User = user,
            Type = GoalType.Project,
            Workspace = workspace
        };
        db.Goals.AddRange(goal, goal2);
        await db.SaveChangesAsync();

        var service = new GoalService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetGoalByIdAsync(user.Id, goal2.Id);

        // Assert
        Assert.Equal(goal2.Id, result.Id);
        Assert.Equal("Second goal", result.Title);
        Assert.Equal(GoalType.Project, result.Type);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task GetGoalByIdAsync_ThrowsNotFoundException_WhenNotExists()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var service = new GoalService(db, new FakeWorkspaceService());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetGoalByIdAsync(userId: 1, goalId: 1)
        );
    }

    [Fact]
    public async Task GetGoalByIdAsync_ThrowsNotFoundException_WhenGoalIsInAnotherWorkspace()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
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
        var goal = new Goal
        {
            Title = "Another Goal",
            IsCompleted = false,
            Type = GoalType.Process,
            User = anotherUser,
            Workspace = anotherWorkspace
        };
        db.Workspaces.Add(userWorkspace);
        db.Goals.Add(goal);
        await db.SaveChangesAsync();
        var service = new GoalService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetGoalByIdAsync(user.Id, goal.Id)
        );
    }

    [Fact]
    public async Task CreateGoalAsync_CreatesGoal_AndReturnsDto()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        db.Workspaces.Add(workspace);
        await db.SaveChangesAsync();
    
        var service = new GoalService(db, new FakeWorkspaceService(workspace.Id));
        var dto = new GoalCreateDto { Title = "New goal", Type = GoalType.Process };

        // Act
        var result = await service.CreateGoalAsync(user.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New goal", result.Title);
        Assert.Equal(GoalType.Process, result.Type);
        Assert.False(result.IsCompleted);

        var goalInDb = await db.Goals.FirstOrDefaultAsync(g => g.Id == result.Id);
        Assert.NotNull(goalInDb);
        Assert.Equal("New goal", goalInDb!.Title);
        Assert.Equal(user.Id, goalInDb.UserId);
        Assert.Equal(GoalType.Process, goalInDb.Type);
        Assert.False(goalInDb.IsCompleted);
    }

    [Fact]
    public async Task CreateGoalAsync_AddsExactlyOneGoal()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        db.Workspaces.Add(workspace);
        await db.SaveChangesAsync();

        var service = new GoalService(db, new FakeWorkspaceService(workspace.Id));
        var dto = new GoalCreateDto { Title = "Goal", Type = GoalType.Process };

        // Act
        await service.CreateGoalAsync(user.Id, dto);

        // Assert
        var goalsCount = await db.Goals.CountAsync();
        Assert.Equal(1, goalsCount);
    }

    [Fact]
    public async Task CreateGoalAsync_AssignsGoalToCorrectUserAndCorrectWorkspace()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user1 = new User { Username = "User 1", PasswordHash = "testhash" };
        var user2 = new User { Username = "User 2", PasswordHash = "testhash" };
        var userWorkspace = new Workspace
        {
            Name = "User Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user1, Role = WorkspaceRole.Owner } }
        };
        var anotherWorkspace = new Workspace
        {
            Name = "Another Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user2, Role = WorkspaceRole.Owner } }
        };
        db.Workspaces.AddRange(userWorkspace, anotherWorkspace);
        await db.SaveChangesAsync();

        var service = new GoalService(db, new FakeWorkspaceService(userWorkspace.Id));
        var dto = new GoalCreateDto { Title = "Goal for user1", Type = GoalType.Process };

        // Act
        var result = await service.CreateGoalAsync(user1.Id, dto);

        // Assert
        var goal = await db.Goals.SingleAsync();
        Assert.Equal(user1.Id, goal.UserId);
        Assert.Equal(userWorkspace.Id, goal.WorkspaceId);
        Assert.NotEqual(user2.Id, goal.UserId);
        Assert.NotEqual(anotherWorkspace.Id, goal.WorkspaceId);
    }

    [Fact]
    public async Task UpdateGoalAsync_UpdatesTitleAndIsCompleted()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var goal = new Goal
        {
            Title = "Original Title",
            IsCompleted = false,
            User = user,
            Type = GoalType.Process,
            Workspace = workspace
        };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalService(db, new FakeWorkspaceService(workspace.Id));

        var dto = new GoalUpdateDto { Title = "  New Title  ", IsCompleted = true };

        // Act
        await service.UpdateGoalAsync(user.Id, goal.Id, dto);

        // Assert
        var updatedGoal = await db.Goals.FindAsync(goal.Id);
        Assert.NotNull(updatedGoal);
        Assert.Equal("New Title", updatedGoal!.Title);
        Assert.True(updatedGoal.IsCompleted);
    }

    [Fact]
    public async Task UpdateGoalAsync_UpdatesOnlyTitle_WhenIsCompletedNull()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var goal = new Goal
        {
            Title = "Original Title",
            IsCompleted = false,
            User = user,
            Type = GoalType.Process,
            Workspace = workspace
        };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalService(db, new FakeWorkspaceService(workspace.Id));

        var dto = new GoalUpdateDto { Title = "Updated Title", IsCompleted = null };

        // Act
        await service.UpdateGoalAsync(user.Id, goal.Id, dto);

        // Assert
        var updatedGoal = await db.Goals.FindAsync(goal.Id);
        Assert.Equal("Updated Title", updatedGoal!.Title);
        Assert.False(updatedGoal.IsCompleted);
    }

    [Fact]
    public async Task UpdateGoalAsync_ThrowsNotFound_WhenGoalDoesNotExist()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var service = new GoalService(db, new FakeWorkspaceService());

        var dto = new GoalUpdateDto { Title = "New Title", IsCompleted = true };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateGoalAsync(userId: 1, goalId: 1, dto)
        );
    }

    [Fact]
    public async Task UpdateGoalAsync_ThrowsNotFoundException_WhenGoalIsInAnotherWorkspace()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
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
        var goal = new Goal
        {
            Title = "Another Goal",
            IsCompleted = false,
            User = anotherUser,
            Type = GoalType.Process,
            Workspace = anotherWorkspace
        };
        db.Workspaces.Add(userWorkspace);
        db.Goals.Add(goal);
        await db.SaveChangesAsync();
        var service = new GoalService(db, new FakeWorkspaceService(userWorkspace.Id));
        var dto = new GoalUpdateDto { Title = "New Title", IsCompleted = true };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateGoalAsync(user.Id, goal.Id, dto)
        );

        var goalInDb = await db.Goals.FindAsync(goal.Id);

        Assert.NotNull(goalInDb);
        Assert.Equal("Another Goal", goalInDb!.Title);
        Assert.False(goalInDb.IsCompleted);
    }

    [Fact]
    public async Task DeleteGoalAsync_DeletesGoal_WhenExists()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var goal = new Goal
        {
            Title = "Goal to delete",
            IsCompleted = false,
            User = user,
            Type = GoalType.Process,
            Workspace = workspace
        };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();
        var service = new GoalService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        await service.DeleteGoalAsync(user.Id, goal.Id);

        // Assert
        var deletedGoal = await db.Goals.FindAsync(goal.Id);
        Assert.Null(deletedGoal);
        Assert.Equal(0, await db.Goals.CountAsync());
    }

    [Fact]
    public async Task DeleteGoalAsync_ThrowsNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var service = new GoalService(db, new FakeWorkspaceService());

        // Act
        var act = async () => await service.DeleteGoalAsync(userId: 1, goalId: 1);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task DeleteGoalAsync_ThrowsNotFoundException_WhenGoalIsInAnotherWorkspace()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
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
        var goal = new Goal
        {
            Title = "Another Goal",
            IsCompleted = false,
            Type = GoalType.Process,
            User = anotherUser,
            Workspace = anotherWorkspace
        };
        db.Workspaces.Add(userWorkspace);
        db.Goals.Add(goal);
        await db.SaveChangesAsync();
        var service = new GoalService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteGoalAsync(user.Id, goal.Id)
        );

        var goalInDb = await db.Goals.FindAsync(goal.Id);
        Assert.NotNull(goalInDb);
    }
}
