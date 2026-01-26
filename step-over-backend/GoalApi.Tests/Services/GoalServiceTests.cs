using GoalApi.Dtos.Goal;
using GoalApi.Services;

namespace GoalApi.Tests.Services;

public class GoalServiceTests
{
    [Fact]
    public async Task GetAllGoalsAsync_ReturnsAllGoals()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        db.Goals.AddRange(
            new Goal { Title = "Goal 1", IsCompleted = false, UserId = user.Id },
            new Goal { Title = "Goal 2", IsCompleted = true, UserId = user.Id }
        );
        await db.SaveChangesAsync();

        var service = new GoalService(db);

        // Act
        var result = await service.GetAllGoalsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Title == "Goal 1" && !g.IsCompleted);
        Assert.Contains(result, g => g.Title == "Goal 2" && g.IsCompleted);
    }

    [Fact]
    public async Task GetGoalByIdAsync_ReturnsGoal_WhenExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var goal = new Goal { Title = "First goal", IsCompleted = false, UserId = user.Id };
        var goal2 = new Goal { Title = "Second goal", IsCompleted = true, UserId = user.Id };
        db.Goals.AddRange(goal, goal2);
        await db.SaveChangesAsync();

        var service = new GoalService(db);

        // Act
        var result = await service.GetGoalByIdAsync(goal2.Id);

        // Assert
        Assert.Equal(goal2.Id, result.Id);
        Assert.Equal("Second goal", result.Title);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task GetGoalByIdAsync_ThrowsNotFoundException_WhenNotExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetGoalByIdAsync(100)
        );
    }

    [Fact]
    public async Task CreateGoalAsync_CreatesGoal_AndReturnsDto()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();
    
        var service = new GoalService(db);
        var dto = new GoalCreateDto { Title = "New goal" };

        // Act
        var result = await service.CreateGoalAsync(user.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New goal", result.Title);
        Assert.False(result.IsCompleted);

        var goalInDb = await db.Goals.FirstOrDefaultAsync(g => g.Id == result.Id);
        Assert.NotNull(goalInDb);
        Assert.Equal("New goal", goalInDb!.Title);
        Assert.Equal(user.Id, goalInDb.UserId);
        Assert.False(goalInDb.IsCompleted);
    }

    [Fact]
    public async Task CreateGoalAsync_AddsExactlyOneGoal()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new GoalService(db);
        var dto = new GoalCreateDto { Title = "Goal" };

        // Act
        await service.CreateGoalAsync(user.Id, dto);

        // Assert
        var goalsCount = await db.Goals.CountAsync();
        Assert.Equal(1, goalsCount);
    }

    [Fact]
    public async Task CreateGoalAsync_AssignsGoalToCorrectUser()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user1 = new User { Username = "User 1", PasswordHash = "testhash" };
        var user2 = new User { Username = "User 2", PasswordHash = "testhash" };
        db.Users.AddRange(user1, user2);
        await db.SaveChangesAsync();

        var service = new GoalService(db);
        var dto = new GoalCreateDto { Title = "Goal for user1" };

        // Act
        var result = await service.CreateGoalAsync(user1.Id, dto);

        // Assert
        var goal = await db.Goals.SingleAsync();
        Assert.Equal(user1.Id, goal.UserId);
        Assert.NotEqual(user2.Id, goal.UserId);
    }

    [Fact]
    public async Task UpdateGoalAsync_UpdatesTitleAndIsCompleted()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var goal = new Goal { Title = "Original Title", IsCompleted = false, UserId = user.Id };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalService(db);

        var dto = new GoalUpdateDto { Title = "  New Title  ", IsCompleted = true };

        // Act
        await service.UpdateGoalAsync(goal.Id, dto);

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
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var goal = new Goal { Title = "Original Title", IsCompleted = false, UserId = user.Id };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalService(db);

        var dto = new GoalUpdateDto { Title = "Updated Title", IsCompleted = null };

        // Act
        await service.UpdateGoalAsync(goal.Id, dto);

        // Assert
        var updatedGoal = await db.Goals.FindAsync(goal.Id);
        Assert.Equal("Updated Title", updatedGoal!.Title);
        Assert.False(updatedGoal.IsCompleted);
    }

    [Fact]
    public async Task UpdateGoalAsync_ThrowsNotFound_WhenGoalDoesNotExist()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalService(db);

        var dto = new GoalUpdateDto { Title = "New Title", IsCompleted = true };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateGoalAsync(100, dto)
        );
    }

    [Fact]
    public async Task DeleteGoalAsync_DeletesGoal_WhenExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalService(db);

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var goal = new Goal { Title = "Goal to delete", IsCompleted = false, UserId = user.Id };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        // Act
        await service.DeleteGoalAsync(goal.Id);

        // Assert
        var deletedGoal = await db.Goals.FindAsync(goal.Id);
        Assert.Null(deletedGoal);
        Assert.Equal(0, await db.Goals.CountAsync());
    }

    [Fact]
    public async Task DeleteGoalAsync_ThrowsNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalService(db);

        // Act
        var act = async () => await service.DeleteGoalAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}
