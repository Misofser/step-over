using GoalApi.Dtos.Habit;
using GoalApi.Services;
using GoalApi.Enums;

namespace GoalApi.Tests.Services;

public class HabitServiceTests
{
    [Fact]
    public async Task GetHabitsByGoalAsync_ShouldReturnHabitsForGoalWithCorrectData()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal1 = new Goal { Title = "Goal 1", IsCompleted = false, Type = GoalType.Process, User = user };
        var goal2 = new Goal { Title = "Goal 2", IsCompleted = true, Type = GoalType.Project, User = user };
        var habitA = new Habit {
            Title = "Habit A",
            Goal = goal1,
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = DateTime.UtcNow.Date } ],
        };
        var habitB = new Habit { Title = "Habit B", Goal = goal1, Frequency = HabitFrequency.Weekly };
        var habitC = new Habit { Title = "Habit C", Goal = goal2, Frequency = HabitFrequency.Daily };

        db.Habits.AddRange(habitA, habitB, habitC);
        await db.SaveChangesAsync();

        var service = new HabitService(db);

        // Act
        var result = await service.GetHabitsByGoalAsync(goal1.Id);

        // Assert
        Assert.Equal(2, result.Count);

        var dtoA = result.Single(h => h.Title == "Habit A");
        var dtoB = result.Single(h => h.Title == "Habit B");

        Assert.Equal(habitA.Id, dtoA.Id);
        Assert.Equal(HabitFrequency.Daily, dtoA.Frequency);
        Assert.True(dtoA.IsCompletedToday);

        Assert.Equal(habitB.Id, dtoB.Id);
        Assert.Equal(HabitFrequency.Weekly, dtoB.Frequency);
        Assert.False(dtoB.IsCompletedToday);
    }

    [Fact]
    public async Task GetHabitsByGoalAsync_ThrowsNotFoundException_WhenGoalNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new HabitService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetHabitsByGoalAsync(1)
        );
    }

    [Fact]
    public async Task GetHabitByIdAsync_ReturnsHabit_WhenExistsAndCompletedToday()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var habitA = new Habit
        {
            Goal = goal,
            Title = "Habit A",
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = DateTime.UtcNow.Date } ],
        };
        var habitB = new Habit { Title = "Habit B", Goal = goal, Frequency = HabitFrequency.Weekly };

        db.Habits.AddRange(habitA, habitB);
        await db.SaveChangesAsync();

        var service = new HabitService(db);

        // Act
        var result = await service.GetHabitByIdAsync(habitA.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(habitA.Id, result.Id);
            Assert.Equal("Habit A", result.Title);
            Assert.Equal(HabitFrequency.Daily, result.Frequency);
            Assert.True(result.IsCompletedToday);
        });
    }

    [Fact]
    public async Task GetHabitByIdAsync_ReturnsHabit_WhenExistsAndNotCompletedToday()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var habitA = new Habit
        {
            Goal = goal,
            Title = "Habit A",
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = DateTime.UtcNow.Date } ],
        };
        var habitB = new Habit { Title = "Habit B", Goal = goal, Frequency = HabitFrequency.Weekly };

        db.Habits.AddRange(habitA, habitB);
        await db.SaveChangesAsync();

        var service = new HabitService(db);

        // Act
        var result = await service.GetHabitByIdAsync(habitB.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(habitB.Id, result.Id);
            Assert.Equal("Habit B", result.Title);
            Assert.Equal(HabitFrequency.Weekly, result.Frequency);
            Assert.False(result.IsCompletedToday);
        });
    }

    [Fact]
    public async Task GetHabitByIdAsync_ThrowsNotFoundException_WhenHabitNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new HabitService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetHabitByIdAsync(1)
        );
    }

    [Fact]
    public async Task AddHabitAsync_ShouldCreateHabit()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new HabitService(db);
        var dto = new HabitCreateDto { Title = "New Habit", Frequency = HabitFrequency.Daily };

        // Act
        var result = await service.AddHabitAsync(goal.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Habit", result.Title);
        Assert.Equal(HabitFrequency.Daily, result.Frequency);
        Assert.False(result.IsCompletedToday);

        var habitInDb = await db.Habits.FindAsync(result.Id);
        Assert.NotNull(habitInDb);
        Assert.Equal(goal.Id, habitInDb!.GoalId);
        Assert.Equal("New Habit", habitInDb.Title);
        Assert.Equal(HabitFrequency.Daily, habitInDb.Frequency);
    }

    [Fact]
    public async Task AddHabitAsync_ThrowsNotFoundException_WhenGoalNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new HabitService(db);
        var dto = new HabitCreateDto { Title = "New Habit", Frequency = HabitFrequency.Daily };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.AddHabitAsync(1, dto)
        );
    }

    [Fact]
    public async Task ToggleCompletion_ShouldAddCompletion_WhenNotExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var habit = new Habit { Title = "Habit", Goal = goal, Frequency = HabitFrequency.Weekly };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();
        var service = new HabitService(db);
        var date = DateTime.UtcNow.Date;

        // Act
        await service.ToggleCompletion(habit.Id, date);

        // Assert
        var exists = await db.HabitCompletions.AnyAsync(c => c.HabitId == habit.Id && c.Date == date);
    
        Assert.True(exists);
    }

    [Fact]
    public async Task ToggleCompletion_ShouldRemoveCompletion_WhenExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var date = DateTime.UtcNow.Date;
        var habit = new Habit
        {
            Goal = goal,
            Title = "Habit",
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = date } ],
        };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();
        var service = new HabitService(db);

        // Act
        await service.ToggleCompletion(habit.Id, date);

        // Assert
        var exists = await db.HabitCompletions.AnyAsync(c => c.HabitId == habit.Id && c.Date == date);

        Assert.False(exists);
    }

    [Fact]
    public async Task ToggleCompletion_ThrowsBadRequestException_WhenDateIsInFuture()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var habit = new Habit { Title = "Habit", Goal = goal, Frequency = HabitFrequency.Weekly };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new HabitService(db);
        var futureDate = DateTime.UtcNow.Date.AddDays(1);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => service.ToggleCompletion(habit.Id, futureDate)
        );
    }

    [Fact]
    public async Task ToggleCompletion_ThrowsNotFoundException_WhenHabitNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new HabitService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.ToggleCompletion(1, DateTime.UtcNow)
        );
    }

    [Fact]
    public async Task DeleteHabitAsync_ShouldRemoveHabit()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal 1", IsCompleted = false, Type = GoalType.Process, User = user };
        var habit = new Habit { Title = "Habit", Goal = goal, Frequency = HabitFrequency.Weekly };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();

        var service = new HabitService(db);

        // Act
        await service.DeleteHabitAsync(habit.Id);

        // Assert
        var deletedHabit = await db.Habits.FindAsync(habit.Id);
        Assert.Null(deletedHabit);
        Assert.Equal(0, await db.Habits.CountAsync());
    }

    [Fact]
    public async Task DeleteHabitAsync_ThrowsNotFoundException_WhenHabitNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new HabitService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteHabitAsync(1)
        );
    }

    [Fact]
    public async Task GetCompletionStatusAsync_ReturnsTrue_WhenCompletionExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var date = DateTime.UtcNow.Date.AddDays(-2);
        var habit = new Habit
        {
            Goal = goal,
            Title = "Habit",
            Frequency = HabitFrequency.Daily,
            Completions = [ new HabitCompletion { Date = date } ],
        };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();
        var service = new HabitService(db);

        // Act
        var result = await service.GetCompletionStatusAsync(habit.Id, date);

        // Assert
        Assert.True(result.IsCompleted);
        Assert.Equal(date, result.Date);
    }

    [Fact]
    public async Task GetCompletionStatusAsync_ReturnsFalse_WhenCompletionDoesNotExist()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var date = DateTime.UtcNow.Date.AddDays(-2);
        var habit = new Habit
        {
            Goal = goal,
            Title = "Habit",
            Frequency = HabitFrequency.Daily,
        };
        db.Habits.Add(habit);
        await db.SaveChangesAsync();
        var service = new HabitService(db);

        // Act
        var result = await service.GetCompletionStatusAsync(habit.Id, date);

        // Assert
        Assert.False(result.IsCompleted);
        Assert.Equal(date, result.Date);
    }

    [Fact]
    public async Task GetCompletionStatusAsync_ThrowsNotFoundException_WhenHabitDoesNotExist()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var date = DateTime.UtcNow.Date;
        var service = new HabitService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetCompletionStatusAsync(1, date)
        );
    }
}
