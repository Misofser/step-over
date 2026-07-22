using GoalApi.Services;
using GoalApi.Enums;

namespace GoalApi.Tests.Services;

public class TodayServiceTests
{
    [Fact]
    public async Task GetTodayItemsAsync_ShouldReturnPendingTask()
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
            User = user,
            Workspace = workspace,
        };
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };

        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TodayService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTodayItemsAsync(user.Id);

        // Assert
        Assert.Empty(result.Completed);
        var item = Assert.Single(result.Pending);
        Assert.Multiple(() =>
        {
            Assert.Equal("Task", item.Title);
            Assert.Equal(TodayItemType.Task, item.Type);
            Assert.False(item.IsCompleted);
            Assert.Equal("Goal", item.GoalTitle);
        });
    }

    [Fact]
    public async Task GetTodayItemsAsync_ShouldReturnTaskCompletedToday()
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
            User = user,
            Workspace = workspace,
        };
        var task = new GoalTask { Title = "Task", IsCompleted = true, Goal = goal, CompletedAt = DateTime.UtcNow };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TodayService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTodayItemsAsync(user.Id);

        // Assert
        Assert.Empty(result.Pending);
        var item = Assert.Single(result.Completed);
        Assert.Multiple(() =>
        {
            Assert.Equal("Task", item.Title);
            Assert.Equal(TodayItemType.Task, item.Type);
            Assert.True(item.IsCompleted);
            Assert.Equal("Goal", item.GoalTitle);
        });
    }

    [Fact]
    public async Task GetTodayItemsAsync_ShouldExcludeTaskCompletedBeforeToday()
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
            User = user,
            Workspace = workspace,
        };
        var task = new GoalTask {
            Title = "Task",
            IsCompleted = true,
            Goal = goal,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TodayService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTodayItemsAsync(user.Id);

        // Assert
        Assert.Empty(result.Pending);
        Assert.Empty(result.Completed);
    }

    [Fact]
    public async Task GetTodayItemsAsync_ShouldReturnCompletedHabit()
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
            User = user,
            Workspace = workspace,
        };
        var habit = new Habit
        {
            Goal = goal,
            Title = "Habit",
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = DateTime.UtcNow.Date } ],
        };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new TodayService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTodayItemsAsync(user.Id);

        // Assert
        Assert.Empty(result.Pending);
        var item = Assert.Single(result.Completed);
        Assert.Multiple(() =>
        {
            Assert.Equal("Habit", item.Title);
            Assert.Equal(TodayItemType.Habit, item.Type);
            Assert.True(item.IsCompleted);
            Assert.Equal("Goal", item.GoalTitle);
        });
    }

    [Fact]
    public async Task GetTodayItemsAsync_ShouldReturnPendingHabit()
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
            User = user,
            Workspace = workspace,
        };
        var habit = new Habit { Goal = goal, Title = "Habit", Frequency = HabitFrequency.Daily };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new TodayService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTodayItemsAsync(user.Id);

        // Assert
        Assert.Empty(result.Completed);
        var item = Assert.Single(result.Pending);
        Assert.Multiple(() =>
        {
            Assert.Equal("Habit", item.Title);
            Assert.Equal(TodayItemType.Habit, item.Type);
            Assert.False(item.IsCompleted);
            Assert.Equal("Goal", item.GoalTitle);
        });
    }

    [Fact]
    public async Task GetTodayItemsAsync_ShouldExcludeItemsFromCompletedGoal()
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
            IsCompleted = true,
            Type = GoalType.Process,
            User = user,
            Workspace = workspace,
        };
        var habit = new Habit { Goal = goal, Title = "Habit", Frequency = HabitFrequency.Daily };
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };
        db.GoalTasks.Add(task);
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new TodayService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTodayItemsAsync(user.Id);

        // Assert
        Assert.Empty(result.Pending);
        Assert.Empty(result.Completed);
    }

    [Fact]
    public async Task GetTodayItemsAsync_ShouldReturnOnlyItemsFromUserWorkspace()
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
        var userGoal = new Goal
        {
            Title = "User Goal",
            IsCompleted = false,
            Type = GoalType.Process,
            Workspace = userWorkspace,
            User = user
        };
        var anotherGoal = new Goal
        {
            Title = "Another Goal",
            IsCompleted = false,
            Type = GoalType.Process,
            Workspace = anotherWorkspace,
            User = anotherUser
        };
        var userTask = new GoalTask
        {
            Title = "User Task",
            IsCompleted = false,
            Goal = userGoal,
            CompletedAt = DateTime.UtcNow
        };
        var anotherTask = new GoalTask
        {
            Title = "Another Task",
            IsCompleted = false,
            Goal = anotherGoal,
            CompletedAt = DateTime.UtcNow
        };
        var userHabit = new Habit { Goal = userGoal, Title = "User Habit", Frequency = HabitFrequency.Daily };
        var anotherHabit = new Habit { Goal = anotherGoal, Title = "Another Habit", Frequency = HabitFrequency.Daily };
        db.GoalTasks.AddRange(userTask, anotherTask);
        db.Habits.AddRange(userHabit, anotherHabit);
        await db.SaveChangesAsync();

        var service = new TodayService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act
        var result = await service.GetTodayItemsAsync(user.Id);

        // Assert
        var titles = result.Pending.Select(x => x.Title).ToList();
        Assert.Contains("User Task", titles);
        Assert.Contains("User Habit", titles);
        Assert.DoesNotContain("Another Task", titles);
        Assert.DoesNotContain("Another Habit", titles);
    }
}
