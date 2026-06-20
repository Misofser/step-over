using GoalApi.Dtos.Today;
using GoalApi.Data;
using GoalApi.Services.Interfaces;
using GoalApi.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Services;

public class TodayService(AppDbContext db) : ITodayService
{
    private readonly AppDbContext _db = db;

    public async Task<TodayDashboardDto> GetTodayItemsAsync()
    {
        var items = await BuildTodayQuery().ToListAsync();

        var (pending, completed) = SplitItems(items);

        return new TodayDashboardDto
        {
            Pending = pending,
            Completed = completed
        };
    }

    private IQueryable<TodayItemDto> BuildTodayQuery()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var habits = _db.Habits
            .Where(h => !h.Goal.IsCompleted)
            .Select(h => new TodayItemDto
            {
                EntityId = h.Id,
                Type = TodayItemType.Habit,
                Title = h.Title,
                IsCompleted = _db.HabitCompletions
                    .Any(c => c.HabitId == h.Id && c.Date == today),
                GoalId = h.GoalId,
                GoalTitle = h.Goal.Title
            });

        var tasks = _db.GoalTasks
            .Where(t => !t.Goal.IsCompleted && (
                !t.IsCompleted || (t.CompletedAt >= today && t.CompletedAt < tomorrow)
            ))
            .Select(t => new TodayItemDto
            {
                EntityId = t.Id,
                Type = TodayItemType.Task,
                Title = t.Title,
                IsCompleted = t.IsCompleted,
                GoalId = t.GoalId,
                GoalTitle = t.Goal.Title
            });

        return habits.Concat(tasks)
            .OrderBy(x => x.IsCompleted)
            .ThenBy(x => x.GoalTitle)
            .ThenBy(x => x.Title);
    }

    private (List<TodayItemDto> pending, List<TodayItemDto> completed) SplitItems(List<TodayItemDto> items)
    {
        var pending = new List<TodayItemDto>();
        var completed = new List<TodayItemDto>();

        foreach (var item in items)
        {
            if (item.IsCompleted)
                completed.Add(item);
            else
                pending.Add(item);
        }

        return (pending, completed);
    }
}
