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
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };

        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TodayService(db);

        // Act
        var result = await service.GetTodayItemsAsync();

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
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var task = new GoalTask { Title = "Task", IsCompleted = true, Goal = goal, CompletedAt = DateTime.UtcNow };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TodayService(db);

        // Act
        var result = await service.GetTodayItemsAsync();

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
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var task = new GoalTask {
            Title = "Task",
            IsCompleted = true,
            Goal = goal,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TodayService(db);

        // Act
        var result = await service.GetTodayItemsAsync();

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
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var habit = new Habit
        {
            Goal = goal,
            Title = "Habit",
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = DateTime.UtcNow.Date } ],
        };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new TodayService(db);

        // Act
        var result = await service.GetTodayItemsAsync();

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
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var habit = new Habit { Goal = goal, Title = "Habit", Frequency = HabitFrequency.Daily };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new TodayService(db);

        // Act
        var result = await service.GetTodayItemsAsync();

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
        var goal = new Goal { Title = "Goal", IsCompleted = true, Type = GoalType.Process, User = user };
        var habit = new Habit { Goal = goal, Title = "Habit", Frequency = HabitFrequency.Daily };
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };
        db.GoalTasks.Add(task);
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new TodayService(db);

        // Act
        var result = await service.GetTodayItemsAsync();

        // Assert
        Assert.Empty(result.Pending);
        Assert.Empty(result.Completed);
    }
}
