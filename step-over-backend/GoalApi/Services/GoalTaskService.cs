using GoalApi.Dtos.GoalTask;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using GoalApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class GoalTaskService(AppDbContext db) : IGoalTaskService
{
    private readonly AppDbContext _db = db;

    public async Task<List<GoalTaskReadDto>> GetTasksByGoalAsync(int goalId)
    {
        var goal = await _db.Goals.FindAsync(goalId);

        if (goal == null) throw new NotFoundException("Goal");
        if (goal.Type != GoalType.Project) throw new BadRequestException("Tasks are supported only for project goals");

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

    public async Task AddTaskAsync(int goalId, GoalTaskCreateDto dto)
    {
        var goal = await _db.Goals.FindAsync(goalId);

        if (goal == null) throw new NotFoundException("Goal");
        if (goal.Type != GoalType.Project) throw new BadRequestException("Tasks are supported only for project goals");

        var task = new GoalTask { GoalId = goalId, Title = dto.Title.Trim() };

        _db.GoalTasks.Add(task);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateCompletionAsync(int taskId, GoalTaskUpdateCompletionDto dto)
    {
        var task = await _db.GoalTasks.FindAsync(taskId);
        if (task == null) throw new NotFoundException("GoalTask");

        task.IsCompleted = dto.IsCompleted!.Value;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        var task = await _db.GoalTasks.FindAsync(taskId);
        if (task == null) throw new NotFoundException("GoalTask");

        _db.GoalTasks.Remove(task);
        await _db.SaveChangesAsync();
    }
}
