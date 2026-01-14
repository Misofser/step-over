using GoalApi.Dtos.Goal;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class GoalService(AppDbContext db, ICurrentUserService currentUser) : IGoalService
{
    private readonly AppDbContext _db = db;
    private readonly ICurrentUserService _currentUser = currentUser;

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

    public async Task<GoalReadDto> GetGoalByIdAsync(int id)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) throw new NotFoundException("Goal");

        return new GoalReadDto
        {
            Id = goal.Id,
            Title = goal.Title,
            IsCompleted = goal.IsCompleted
        };
    }

    public async Task<GoalReadDto> CreateGoalAsync(GoalCreateDto dto)
    {
        var userId = _currentUser.GetUserId();

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

    public async Task UpdateGoalAsync(int goalId, GoalUpdateDto dto)
    {
        var goal = await _db.Goals.FindAsync(goalId);
        if (goal == null) throw new NotFoundException("Goal");

        if (!string.IsNullOrWhiteSpace(dto.Title))
            goal.Title = dto.Title.Trim();

        if (dto.IsCompleted.HasValue)
            goal.IsCompleted = dto.IsCompleted.Value;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteGoalAsync(int id)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) throw new NotFoundException("Goal");

        _db.Goals.Remove(goal);
        await _db.SaveChangesAsync();
    }
}
