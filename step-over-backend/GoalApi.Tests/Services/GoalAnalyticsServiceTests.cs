using GoalApi.Services;
using GoalApi.Enums;

namespace GoalApi.Tests.Services;

public class GoalAnalyticsServiceTests
{
    [Fact]
    public async Task GetGoalHeatmapAsync_ShouldReturnCorrectData()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var goal = new Goal
        {
            Title = "Goal",
            IsCompleted = false,
            Type = GoalType.Process,
            Workspace = workspace,
            User = user,
        };
        var today = DateTime.UtcNow.Date;
        var twoDaysAgo = today.AddDays(-2);
        var habitA = new Habit {
            Title = "Habit A",
            Goal = goal,
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = today }, new HabitCompletion { Date = twoDaysAgo }, ],
        };
        var habitB = new Habit {
            Title = "Habit B",
            Goal = goal,
            Frequency = HabitFrequency.Weekly,
            Completions = [ new HabitCompletion { Date = today } ],
        };

        db.Habits.AddRange(habitA, habitB);
        await db.SaveChangesAsync();

        var service = new GoalAnalyticsService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetGoalHeatmapAsync(user.Id, goal.Id, 7);

        // Assert
        Assert.Equal(7, result.Count);

        var todayEntry = result.Single(x => x.Date == today);

        Assert.Equal(2, todayEntry.TotalHabits);
        Assert.Equal(2, todayEntry.CompletedHabits);

        var twoDaysAgoEntry = result.Single(x => x.Date == twoDaysAgo);

        Assert.Equal(2, twoDaysAgoEntry.TotalHabits);
        Assert.Equal(1, twoDaysAgoEntry.CompletedHabits);

        var emptyDay = result.Single(x => x.Date == today.AddDays(-1));

        Assert.Equal(2, emptyDay.TotalHabits);
        Assert.Equal(0, emptyDay.CompletedHabits);
    }

    [Fact]
    public async Task GetGoalHeatmapAsync_ShouldReturnEmptyHeatmap_WhenGoalHasNoHabits()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var goal = new Goal
        {
            Title = "Goal",
            IsCompleted = false,
            Type = GoalType.Process,
            Workspace = workspace,
            User = user,
        };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalAnalyticsService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetGoalHeatmapAsync(user.Id, goal.Id, 7);

        // Assert
        Assert.Equal(7, result.Count);

        Assert.All(result, day =>
        {
            Assert.Equal(0, day.CompletedHabits);
            Assert.Equal(0, day.TotalHabits);
        });
    }

    [Fact]
    public async Task GetGoalHeatmapAsync_ThrowsNotFoundException_WhenGoalNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var service = new GoalAnalyticsService(db, new FakeWorkspaceService());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetGoalHeatmapAsync(userId: 1, goalId: 1, days: 7)
        );
    }

    [Fact]
    public async Task GetGoalHeatmapAsync_ThrowsNotFoundException_WhenGoalBelongsToAnotherWorkspace()
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
        var goal = new Goal
        {
            Title = "Other workspace goal",
            IsCompleted = false,
            Type = GoalType.Process,
            Workspace = anotherWorkspace,
            User = anotherUser,
        };

        db.Workspaces.Add(userWorkspace);
        db.Goals.Add(goal);

        await db.SaveChangesAsync();

        var service = new GoalAnalyticsService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetGoalHeatmapAsync(user.Id, goal.Id, 7)
        );
    }
}
