using GoalApi.Dtos.Goal;
using GoalApi.Data;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class GoalAnalyticsService(AppDbContext db, IWorkspaceService workspaceService) : IGoalAnalyticsService
{
    private readonly AppDbContext _db = db;
    private readonly IWorkspaceService _workspaceService = workspaceService;

    public async Task<List<GoalHeatmapDto>> GetGoalHeatmapAsync(int userId, int goalId, int days = 30)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);

        await EnsureGoalExistsAsync(goalId, workspaceId);

        var today = DateTime.UtcNow.Date;
        var fromDate = today.AddDays(-(days - 1));

        var habitIds = await GetHabitIdsAsync(goalId);

        if (!habitIds.Any()) return CreateEmptyHeatmap(fromDate, days);

        var grouped = await GetCompletionCountsAsync(habitIds, fromDate, today);

        return Enumerable.Range(0, days)
            .Select(dayIndex =>
            {
                var date = fromDate.AddDays(dayIndex);
                grouped.TryGetValue(date, out var completedCount);

                return new GoalHeatmapDto
                {
                    Date = date,
                    CompletedHabits = completedCount,
                    TotalHabits = habitIds.Count
                };
            })
            .ToList();
    }

    private async Task EnsureGoalExistsAsync(int goalId, int workspaceId)
    {
        if (!await _db.Goals.AnyAsync(g =>
                g.Id == goalId &&
                g.WorkspaceId == workspaceId))
        {
            throw new NotFoundException("Goal");
        }
    }

    private Task<List<int>> GetHabitIdsAsync(int goalId)
    {
        return _db.Habits
            .Where(h => h.GoalId == goalId)
            .Select(h => h.Id)
            .ToListAsync();
    }

    private List<GoalHeatmapDto> CreateEmptyHeatmap(DateTime fromDate, int days)
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

    private async Task<Dictionary<DateTime, int>> GetCompletionCountsAsync(
        List<int> habitIds,
        DateTime fromDate,
        DateTime toDate)
    {
        return await _db.HabitCompletions
            .Where(c =>
                habitIds.Contains(c.HabitId) &&
                c.Date >= fromDate &&
                c.Date <= toDate)
            .GroupBy(c => c.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Date, x => x.Count);
    }
}
