using GoalApi.Dtos.GoalTask;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using GoalApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class GoalTaskService(AppDbContext db, IWorkspaceService workspaceService) : IGoalTaskService
{
    private readonly AppDbContext _db = db;
    private readonly IWorkspaceService _workspaceService = workspaceService;

    public async Task<List<GoalTaskReadDto>> GetTasksByGoalAsync(int userId, int goalId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        await EnsureGoalExistsAsync(goalId, workspaceId);

        return await _db.GoalTasks
            .Where(t => t.GoalId == goalId)
            .Select(t => new GoalTaskReadDto
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsCompleted
            })
            .ToListAsync();
    }

    public async Task<GoalTaskReadDto> GetTaskByIdAsync(int userId, int taskId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var task = await GetTaskOrThrowAsync(taskId, workspaceId);

        return new GoalTaskReadDto
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted
        };
    }

    public async Task<GoalTaskReadDto> AddTaskAsync(int userId, int goalId, GoalTaskCreateDto dto)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        await EnsureGoalExistsAsync(goalId, workspaceId);

        var task = new GoalTask { GoalId = goalId, Title = dto.Title.Trim() };

        _db.GoalTasks.Add(task);
        await _db.SaveChangesAsync();

        return new GoalTaskReadDto
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted,
        };
    }

    public async Task UpdateCompletionAsync(int userId, int taskId, GoalTaskUpdateCompletionDto dto)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var task = await GetTaskOrThrowAsync(taskId, workspaceId);
        var isCompleted = dto.IsCompleted!.Value;

        task.IsCompleted = isCompleted;
        task.CompletedAt = isCompleted ? DateTime.UtcNow : null;

        await _db.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(int userId, int taskId, GoalTaskUpdateDto dto)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var task = await GetTaskOrThrowAsync(taskId, workspaceId);

        if (!string.IsNullOrWhiteSpace(dto.Title))
            task.Title = dto.Title.Trim();

        await _db.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int userId, int taskId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var task = await GetTaskOrThrowAsync(taskId, workspaceId);

        _db.GoalTasks.Remove(task);
        await _db.SaveChangesAsync();
    }

    private async Task EnsureGoalExistsAsync(int goalId, int workspaceId)
    {
        var exists = await _db.Goals.AnyAsync(g => g.Id == goalId && g.WorkspaceId == workspaceId);
        if (!exists) throw new NotFoundException("Goal");
    }

    private async Task<GoalTask> GetTaskOrThrowAsync(int taskId, int workspaceId)
    {
        var task = await _db.GoalTasks.SingleOrDefaultAsync(t => t.Id == taskId && t.Goal.WorkspaceId == workspaceId);
        if (task == null) throw new NotFoundException("GoalTask");
        return task;
    }
}
