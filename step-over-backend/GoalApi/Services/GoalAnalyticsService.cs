using GoalApi.Dtos.Goal;
using GoalApi.Data;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class GoalAnalyticsService(AppDbContext db) : IGoalAnalyticsService
{
    private readonly AppDbContext _db = db;

    public async Task<List<GoalHeatmapDto>> GetGoalHeatmapAsync(int goalId, int days = 30)
    {
        var goal = await _db.Goals.FindAsync(goalId);
        if (goal == null) throw new NotFoundException("Goal");

        var today = DateTime.UtcNow.Date;
        var fromDate = today.AddDays(-(days - 1));

        var habitIds = await _db.Habits
            .Where(h => h.GoalId == goalId)
            .Select(h => h.Id)
            .ToListAsync();

        if (!habitIds.Any())
        {
            return Enumerable.Range(0, days)
                .Select(dayIndex => new GoalHeatmapDto
                {
                    Date = fromDate.AddDays(dayIndex),
                    CompletedHabits = 0,
                    TotalHabits = 0
                })
                .ToList();
        }

        var grouped = await _db.HabitCompletions
            .Where(c =>
                habitIds.Contains(c.HabitId) &&
                c.Date >= fromDate &&
                c.Date <= today)
            .GroupBy(c => c.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Date, x => x.Count);

        var result = Enumerable.Range(0, days)
            .Select(dayIndex =>
            {
                var date = fromDate.AddDays(dayIndex);
                grouped.TryGetValue(date, out var completedCount);

                return new GoalHeatmapDto
                {
                    Date = date,
                    CompletedHabits = completedCount,
                    TotalHabits = habitIds.Count()
                };
            })
            .ToList();

        return result;
    }
}
