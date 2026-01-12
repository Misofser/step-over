using GoalApi.Dtos.Goal;
using GoalApi.Data;
using GoalApi.Models;
using Microsoft.EntityFrameworkCore;

public class GoalService(AppDbContext db) : IGoalService
{
    private readonly AppDbContext _db = db;

    public async Task<List<GoalReadDto>> GetAllGoalsAsync()
    {
        return await _db.Goals
            .Select(g => new GoalReadDto
            {
                Id = g.Id,
                Title = g.Title,
                IsCompleted = g.IsCompleted,
            })
            .ToListAsync();
    }

    public async Task<GoalReadDto?> GetGoalByIdAsync(int id)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) return null;

        return new GoalReadDto
        {
            Id = goal.Id,
            Title = goal.Title,
            IsCompleted = goal.IsCompleted
        };
    }

    public async Task<GoalReadDto> CreateGoalAsync(int userId, GoalCreateDto dto)
    {
        var goal = new Goal
        {
            Title = dto.Title,
            UserId = userId
        };

        _db.Goals.Add(goal);
        await _db.SaveChangesAsync();

        return new GoalReadDto
        {
            Id = goal.Id,
            Title = goal.Title,
            IsCompleted = goal.IsCompleted
        };
    }

    public async Task<bool> UpdateGoalAsync(int goalId, GoalUpdateDto dto)
    {
        var goal = await _db.Goals.FindAsync(goalId);
        if (goal == null) return false;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            goal.Title = dto.Title.Trim();

        if (dto.IsCompleted.HasValue)
            goal.IsCompleted = dto.IsCompleted.Value;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGoalAsync(int id)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) return false;

        _db.Goals.Remove(goal);
        await _db.SaveChangesAsync();
        return true;
    }
}
