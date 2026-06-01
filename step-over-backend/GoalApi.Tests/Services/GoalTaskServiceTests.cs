using GoalApi.Dtos.GoalTask;
using GoalApi.Services;
using GoalApi.Enums;

namespace GoalApi.Tests.Services;

public class GoalTaskServiceTests
{
    [Fact]
    public async Task GetTasksByGoalAsync_ShouldReturnTasksForGoal()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal1 = new Goal { Title = "Goal 1", IsCompleted = false, Type = GoalType.Process, User = user };
        var goal2 = new Goal { Title = "Goal 2", IsCompleted = true, Type = GoalType.Project, User = user };

        db.GoalTasks.AddRange(
            new GoalTask { Title = "Task A", IsCompleted = false, Goal = goal1 },
            new GoalTask { Title = "Task B", IsCompleted = true, Goal = goal1 },
            new GoalTask { Title = "Task C", IsCompleted = true, Goal = goal2 }
        );

        await db.SaveChangesAsync();
        var service = new GoalTaskService(db);

        // Act
        var result = await service.GetTasksByGoalAsync(1);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Title == "Task A" && !t.IsCompleted);
        Assert.Contains(result, t => t.Title == "Task B" && t.IsCompleted);
    }

    [Fact]
    public async Task GetTasksByGoalAsync_ShouldReturnEmptyList_WhenNoTasks()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, User = user, Type = GoalType.Process };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db);

        // Act
        var result = await service.GetTasksByGoalAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTasksByGoalAsync_ThrowsNotFoundException_WhenGoalNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetTasksByGoalAsync(1)
        );
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsGoalTask_WhenExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal 1", IsCompleted = false, Type = GoalType.Process, User = user };
        var task1 = new GoalTask { Title = "Task A", IsCompleted = false, Goal = goal };
        var task2 = new GoalTask { Title = "Task B", IsCompleted = true, Goal = goal };
        db.GoalTasks.AddRange(task1, task2);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db);

        // Act
        var result = await service.GetTaskByIdAsync(task2.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(task2.Id, result.Id);
            Assert.Equal("Task B", result.Title);
            Assert.True(result.IsCompleted);
        });
    }

    [Fact]
    public async Task GetTaskByIdAsync_ThrowsNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetTaskByIdAsync(1)
        );
    }

    [Fact]
    public async Task AddTaskAsync_ShouldCreateTask()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db);
        var dto = new GoalTaskCreateDto { Title = "  New Task  " };

        // Act
        var result = await service.AddTaskAsync(goal.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Task", result.Title);
        Assert.False(result.IsCompleted);

        var taskInDb = await db.GoalTasks.FindAsync(result.Id);
        Assert.NotNull(taskInDb);
        Assert.Equal(goal.Id, taskInDb!.GoalId);
        Assert.Equal("New Task", taskInDb.Title);
        Assert.False(taskInDb.IsCompleted);
    }

    [Fact]
    public async Task AddTaskAsync_ThrowsNotFoundException_WhenGoalDoesNotExist()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db);

        var dto = new GoalTaskCreateDto { Title = "New Task" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.AddTaskAsync(1, dto)
        );
    }

    [Fact]
    public async Task AddTaskAsync_AddsExactlyOneTask()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db);
        var dto = new GoalTaskCreateDto { Title = "New Task" };

        // Act
        await service.AddTaskAsync(goal.Id, dto);

        // Assert
        var tasksCount = await db.GoalTasks.CountAsync();
        Assert.Equal(1, tasksCount);
    }

    [Fact]
    public async Task UpdateCompletionAsync_ShouldUpdateIsCompleted()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var task = new GoalTask { Title = "Task A", IsCompleted = false, Goal = goal };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db);
        var dto = new GoalTaskUpdateCompletionDto { IsCompleted = true };

        // Act
        await service.UpdateCompletionAsync(task.Id, dto);

        // Assert
        var updatedTask = await db.GoalTasks.FindAsync(task.Id);

        Assert.NotNull(updatedTask);
        Assert.True(updatedTask!.IsCompleted);
    }

    [Fact]
    public async Task UpdateCompletionAsync_ThrowsNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db);

        var dto = new GoalTaskUpdateCompletionDto { IsCompleted = true };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateCompletionAsync(1, dto)
        );
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTitle()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal", IsCompleted = false, Type = GoalType.Process, User = user };
        var task = new GoalTask { Title = "Old Title", IsCompleted = false, Goal = goal };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db);
        var dto = new GoalTaskUpdateDto { Title = "   New Title" };

        // Act
        await service.UpdateTaskAsync(task.Id, dto);

        // Assert
        var updatedTask = await db.GoalTasks.FindAsync(task.Id);

        Assert.NotNull(updatedTask);
        Assert.Equal("New Title", updatedTask!.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_ThrowsNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db);
        var dto = new GoalTaskUpdateDto { Title = "New Title" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateTaskAsync(1, dto)
        );
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTask()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var goal = new Goal { Title = "Goal 1", IsCompleted = false, Type = GoalType.Process, User = user };
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db);

        // Act
        await service.DeleteTaskAsync(task.Id);

        // Assert
        var deletedTask = await db.GoalTasks.FindAsync(task.Id);
        Assert.Null(deletedTask);
        Assert.Equal(0, await db.GoalTasks.CountAsync());
    }

    [Fact]
    public async Task DeleteTaskAsync_ThrowsNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteTaskAsync(1)
        );
    }
}
