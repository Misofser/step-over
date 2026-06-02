using GoalApi.Services;
using GoalApi.Enums;

namespace GoalApi.Tests.Services;

public class GoalAnalyticsServiceTests
{
    [Fact]
    public async Task GetGoalHeatmapAsync_ShouldReturnCorectData()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
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

        var service = new GoalAnalyticsService(db);

        // Act
        var result = await service.GetGoalHeatmapAsync(goal.Id, 7);

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
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalAnalyticsService(db);

        // Act
        var result = await service.GetGoalHeatmapAsync(goal.Id, 7);

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
        var service = new GoalAnalyticsService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetGoalHeatmapAsync(1, 7)
        );
    }
}
