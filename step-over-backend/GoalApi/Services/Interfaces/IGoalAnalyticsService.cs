using GoalApi.Dtos.Goal;

namespace GoalApi.Services.Interfaces;

public interface IGoalAnalyticsService
{
    Task<List<GoalHeatmapDto>> GetGoalHeatmapAsync(int userId, int goalId, int days);
}
