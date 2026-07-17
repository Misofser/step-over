using GoalApi.Dtos.Goal;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Enums;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class GoalService(AppDbContext db, IWorkspaceService workspaceService) : IGoalService
{
    private readonly AppDbContext _db = db;
    private readonly IWorkspaceService _workspaceService = workspaceService;

    public async Task<List<GoalReadDto>> GetAllGoalsAsync(int userId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);

        return await _db.Goals
            .Where(g => g.WorkspaceId == workspaceId)
            .Select(g => new GoalReadDto
            {
                Id = g.Id,
                Title = g.Title,
                IsCompleted = g.IsCompleted,
                Type = g.Type
            })
            .ToListAsync();
    }

    public async Task<GoalReadDto> GetGoalByIdAsync(int userId, int goalId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var goal = await GetGoalAsync(goalId, workspaceId);
        return new GoalReadDto
        {
            Id = goal.Id,
            Title = goal.Title,
            IsCompleted = goal.IsCompleted,
            Type = goal.Type
        };
    }

    public async Task<GoalReadDto> CreateGoalAsync(int userId, GoalCreateDto dto)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var goal = new Goal
        {
            Title = dto.Title,
            UserId = userId,
            WorkspaceId = workspaceId,
            Type = dto.Type
        };

        _db.Goals.Add(goal);
        await _db.SaveChangesAsync();

        return new GoalReadDto
        {
            Id = goal.Id,
            Title = goal.Title,
            IsCompleted = goal.IsCompleted,
            Type = goal.Type
        };
    }

    public async Task UpdateGoalAsync(int userId, int goalId, GoalUpdateDto dto)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var goal = await GetGoalAsync(goalId, workspaceId);

        if (!string.IsNullOrWhiteSpace(dto.Title))
            goal.Title = dto.Title.Trim();

        if (dto.IsCompleted.HasValue)
            goal.IsCompleted = dto.IsCompleted.Value;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteGoalAsync(int userId, int goalId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var goal = await GetGoalAsync(goalId, workspaceId);

        _db.Goals.Remove(goal);
        await _db.SaveChangesAsync();
    }

    private async Task<Goal> GetGoalAsync(int goalId, int workspaceId)
    {
        var goal = await _db.Goals.SingleOrDefaultAsync(g => g.Id == goalId && g.WorkspaceId == workspaceId);
        if (goal == null) throw new NotFoundException("Goal");

        return goal;
    }
}
