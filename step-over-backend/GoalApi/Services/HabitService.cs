using GoalApi.Dtos.Habit;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using GoalApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class HabitService(AppDbContext db) : IHabitService
{
    private readonly AppDbContext _db = db;

    public async Task<List<HabitReadDto>> GetHabitsByGoalAsync(int goalId)
    {
        await EnsureGoalExistsAsync(goalId);

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

    public async Task<HabitReadDto> GetHabitByIdAsync(int habitId)
    {
        var habit = await _db.Habits.FindAsync(habitId);
        if (habit == null) throw new NotFoundException("Habit");

        var today = DateTime.UtcNow.Date;

        return new HabitReadDto
        {
            Id = habit.Id,
            Title = habit.Title,
            Frequency = habit.Frequency,
            IsCompletedToday = habit.Completions.Any(c => c.Date == today)
        };
    }

    public async Task<HabitReadDto> AddHabitAsync(int goalId, HabitCreateDto dto)
    {
        await EnsureGoalExistsAsync(goalId);

        var today = DateTime.UtcNow.Date;

        var habit = new Habit
        {
            GoalId = goalId,
            Title = dto.Title,
            Frequency = dto.Frequency
        };

        _db.Habits.Add(habit);
        await _db.SaveChangesAsync();

        return new HabitReadDto
        {
            Id = habit.Id,
            Title = habit.Title,
            Frequency = habit.Frequency,
            IsCompletedToday = habit.Completions.Any(c => c.Date == today)
        };
    }

    public async Task ToggleCompletion(int habitId, DateTime date)
    {
        var habit = await _db.Habits.FindAsync(habitId);
        if (habit == null) throw new NotFoundException("Habit");

        date = date.Date;

        if (date > DateTime.UtcNow.Date) throw new BadRequestException("Cannot mark future dates");

        var existing = await _db.HabitCompletions
            .FirstOrDefaultAsync(c => c.HabitId == habitId && c.Date == date);

        if (existing != null) {
            _db.HabitCompletions.Remove(existing);
        }
        else
        {
            var completion = new HabitCompletion
            {
                HabitId = habitId,
                Date = date,
            };

            _db.HabitCompletions.Add(completion);
        }

        await _db.SaveChangesAsync();
    }

    private async Task EnsureGoalExistsAsync(int goalId)
    {
        if (!await _db.Goals.AnyAsync(g => g.Id == goalId)) throw new NotFoundException("Goal");
    }
}
