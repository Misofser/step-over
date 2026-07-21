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
        var workspace = new Workspace
        {
            Name = "Test Workspace",
            Type = WorkspaceType.Personal,
            Members = { new WorkspaceMember { User = user, Role = WorkspaceRole.Owner } }
        };
        var goal1 = new Goal
        {
            Title = "Goal 1",
            IsCompleted = false,
            Type = GoalType.Process,
            User = user,
            Workspace = workspace,
        };
        var goal2 = new Goal
        {
            Title = "Goal 2",
            IsCompleted = true,
            Type = GoalType.Project,
            User = user,
            Workspace = workspace,
        };
        db.GoalTasks.AddRange(
            new GoalTask { Title = "Task A", IsCompleted = false, Goal = goal1 },
            new GoalTask { Title = "Task B", IsCompleted = true, Goal = goal1 },
            new GoalTask { Title = "Task C", IsCompleted = true, Goal = goal2 }
        );

        await db.SaveChangesAsync();
        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTasksByGoalAsync(user.Id, goal1.Id);

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
            User = user,
            Type = GoalType.Process,
            Workspace = workspace,
        };
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTasksByGoalAsync(user.Id, goal.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTasksByGoalAsync_ThrowsNotFoundException_WhenGoalNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db, new FakeWorkspaceService());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetTasksByGoalAsync(userId: 1, goalId: 1)
        );
    }

    [Fact]
    public async Task GetTasksByGoalAsync_ThrowsNotFoundException_WhenGoalIsInAnotherWorkspace()
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
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };
        db.Workspaces.Add(userWorkspace);
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetTasksByGoalAsync(user.Id, goal.Id)
        );
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsGoalTask_WhenExists()
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
            User = user,
            Type = GoalType.Process,
            Workspace = workspace,
        };
        var task1 = new GoalTask { Title = "Task A", IsCompleted = false, Goal = goal };
        var task2 = new GoalTask { Title = "Task B", IsCompleted = true, Goal = goal };
        db.GoalTasks.AddRange(task1, task2);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        var result = await service.GetTaskByIdAsync(user.Id, task2.Id);

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
        var service = new GoalTaskService(db, new FakeWorkspaceService());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetTaskByIdAsync(userId: 1, taskId: 1)
        );
    }

    [Fact]
    public async Task GetTaskByIdAsync_ThrowNotFoundException_WhenTaskIsInAnotherWorkspace()
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
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetTaskByIdAsync(user.Id, task.Id)
        );
    }

    [Fact]
    public async Task AddTaskAsync_ShouldCreateTask()
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
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));
        var dto = new GoalTaskCreateDto { Title = "  New Task  " };

        // Act
        var result = await service.AddTaskAsync(user.Id, goal.Id, dto);

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
        var service = new GoalTaskService(db, new FakeWorkspaceService());

        var dto = new GoalTaskCreateDto { Title = "New Task" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.AddTaskAsync(userId: 1, goalId: 1, dto)
        );
    }

    [Fact]
    public async Task AddTaskAsync_AddsExactlyOneTask()
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
        db.Goals.Add(goal);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));
        var dto = new GoalTaskCreateDto { Title = "New Task" };

        // Act
        await service.AddTaskAsync(user.Id, goal.Id, dto);

        // Assert
        var tasksCount = await db.GoalTasks.CountAsync();
        Assert.Equal(1, tasksCount);
    }

    [Fact]
    public async Task AddTaskAsync_ThrowsNotFoundException_WhenGoalIsInAnotherWorkspace()
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
        var service = new GoalTaskService(db, new FakeWorkspaceService(userWorkspace.Id));
        var dto = new GoalTaskCreateDto { Title = "  New Task  " };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.AddTaskAsync(user.Id, goal.Id, dto)
        );
    }

    [Fact]
    public async Task UpdateCompletionAsync_ShouldUpdateIsCompleted()
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

        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));
        var dto = new GoalTaskUpdateCompletionDto { IsCompleted = true };

        // Act
        await service.UpdateCompletionAsync(user.Id, task.Id, dto);

        // Assert
        var updatedTask = await db.GoalTasks.FindAsync(task.Id);

        Assert.NotNull(updatedTask);
        Assert.True(updatedTask!.IsCompleted);
        Assert.NotNull(updatedTask.CompletedAt);

        Assert.True(
            updatedTask.CompletedAt <= DateTime.UtcNow &&
            updatedTask.CompletedAt > DateTime.UtcNow.AddMinutes(-1)
        );
    }

    [Fact]
    public async Task UpdateCompletionAsync_ThrowsNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new GoalTaskService(db, new FakeWorkspaceService());

        var dto = new GoalTaskUpdateCompletionDto { IsCompleted = true };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateCompletionAsync(userId: 1, taskId: 1, dto)
        );
    }

    [Fact]
    public async Task UpdateCompletionAsync_ThrowsNotFoundException_WhenTaskIsInAnotherWorkspace()
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
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };
        db.Workspaces.Add(userWorkspace);
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();
        var service = new GoalTaskService(db, new FakeWorkspaceService(userWorkspace.Id));
        var dto = new GoalTaskUpdateCompletionDto { IsCompleted = true };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateCompletionAsync(user.Id, task.Id, dto)
        );

        var taskInDb = await db.GoalTasks.FindAsync(task.Id);

        Assert.NotNull(taskInDb);
        Assert.False(taskInDb!.IsCompleted);
        Assert.Null(taskInDb.CompletedAt);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTitle()
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
        var task = new GoalTask { Title = "Old Title", IsCompleted = false, Goal = goal };
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));
        var dto = new GoalTaskUpdateDto { Title = "   New Title" };

        // Act
        await service.UpdateTaskAsync(user.Id, task.Id, dto);

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
        var service = new GoalTaskService(db, new FakeWorkspaceService());
        var dto = new GoalTaskUpdateDto { Title = "New Title" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateTaskAsync(userId: 1, taskId: 1, dto)
        );
    }

    [Fact]
    public async Task UpdateTaskAsync_ThrowsNotFoundException_WhenTaskIsInAnotherWorkspace()
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
        var task = new GoalTask { Title = "Old Title", IsCompleted = false, Goal = goal };
        db.Workspaces.Add(userWorkspace);
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(userWorkspace.Id));
        var dto = new GoalTaskUpdateDto { Title = "New Title" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateTaskAsync(user.Id, task.Id, dto)
        );

        var taskInDb = await db.GoalTasks.FindAsync(task.Id);
        Assert.NotNull(taskInDb);
        Assert.Equal("Old Title", taskInDb!.Title);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTask()
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

        var service = new GoalTaskService(db, new FakeWorkspaceService(workspace.Id));

        // Act
        await service.DeleteTaskAsync(user.Id, task.Id);

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
        var service = new GoalTaskService(db, new FakeWorkspaceService());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteTaskAsync(userId: 1, taskId: 1)
        );
    }

    [Fact]
    public async Task DeleteTaskAsync_ThrowsNotFoundException_WhenTaskIsInAnotherWorkspace()
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
        var task = new GoalTask { Title = "Task", IsCompleted = false, Goal = goal };

        db.Workspaces.Add(userWorkspace);
        db.GoalTasks.Add(task);
        await db.SaveChangesAsync();

        var service = new GoalTaskService(db, new FakeWorkspaceService(userWorkspace.Id));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteTaskAsync(user.Id, task.Id)
        );
        var taskInDb = await db.GoalTasks.FindAsync(task.Id);
        Assert.NotNull(taskInDb);
    }
}
