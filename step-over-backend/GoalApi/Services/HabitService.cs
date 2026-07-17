using GoalApi.Dtos.Habit;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using GoalApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class HabitService(AppDbContext db, IWorkspaceService workspaceService) : IHabitService
{
    private readonly AppDbContext _db = db;
    private readonly IWorkspaceService _workspaceService = workspaceService;

    public async Task<List<HabitReadDto>> GetHabitsByGoalAsync(int userId, int goalId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        await EnsureGoalExistsAsync(goalId, workspaceId);

        var today = DateTime.UtcNow.Date;

        return await _db.Habits
            .Where(h => h.GoalId == goalId)
            .Select(h => new HabitReadDto
            {
                Id = h.Id,
                Title = h.Title,
                Frequency = h.Frequency,
                IsCompletedToday = h.Completions.Any(c => c.Date == today)
            })
            .ToListAsync();
    }

    public async Task<HabitReadDto> GetHabitByIdAsync(int userId, int habitId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var today = DateTime.UtcNow.Date;
        var habit = await _db.Habits
            .Where(h => h.Id == habitId && h.Goal.WorkspaceId == workspaceId)
            .Select(h => new HabitReadDto
            {
                Id = h.Id,
                Title = h.Title,
                Frequency = h.Frequency,
                IsCompletedToday = h.Completions.Any(c => c.Date == today)
            })
            .SingleOrDefaultAsync();

        if (habit == null) throw new NotFoundException("Habit");

        return habit;
    }

    public async Task<HabitReadDto> AddHabitAsync(int userId, int goalId, HabitCreateDto dto)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        await EnsureGoalExistsAsync(goalId, workspaceId);

        var habit = new Habit { GoalId = goalId, Title = dto.Title.Trim(), Frequency = dto.Frequency };
        _db.Habits.Add(habit);
        await _db.SaveChangesAsync();

        return new HabitReadDto
        {
            Id = habit.Id,
            Title = habit.Title,
            Frequency = habit.Frequency,
            IsCompletedToday = false
        };
    }

    public async Task ToggleCompletion(int userId, int habitId, DateTime date)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);

        var habitExists = await _db.Habits.AnyAsync(h => h.Id == habitId && h.Goal.WorkspaceId == workspaceId);
        if (!habitExists) throw new NotFoundException("Habit");

        date = date.Date;

        if (date > DateTime.UtcNow.Date) throw new BadRequestException("Cannot mark future dates");

        var existing = await _db.HabitCompletions
            .FirstOrDefaultAsync(c => c.HabitId == habitId && c.Date == date);

        if (existing != null)
        {
            _db.HabitCompletions.Remove(existing);
        }
        else
        {
            _db.HabitCompletions.Add(new HabitCompletion
            {
                HabitId = habitId,
                Date = date
            });
        }

        await _db.SaveChangesAsync();
    }

    public async Task<HabitCompletionStatusDto> GetCompletionStatusAsync(int userId, int habitId, DateTime date)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);

        date = date.Date;

        var result = await _db.Habits
            .Where(h => h.Id == habitId && h.Goal.WorkspaceId == workspaceId)
            .Select(h => new HabitCompletionStatusDto
            {
                Date = date,
                IsCompleted = h.Completions.Any(c => c.Date == date)
            })
            .SingleOrDefaultAsync();

        if (result == null) throw new NotFoundException("Habit");

        return result;
    }

    public async Task DeleteHabitAsync(int userId, int habitId)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var habit = await _db.Habits.SingleOrDefaultAsync(h => h.Id == habitId && h.Goal.WorkspaceId == workspaceId);
        if (habit == null) throw new NotFoundException("Habit");

        _db.Habits.Remove(habit);
        await _db.SaveChangesAsync();
    }

    private async Task EnsureGoalExistsAsync(int goalId, int workspaceId)
    {
        var exists = await _db.Goals.AnyAsync(g => g.Id == goalId && g.WorkspaceId == workspaceId);
        if (!exists) throw new NotFoundException("Goal");
    }
}
