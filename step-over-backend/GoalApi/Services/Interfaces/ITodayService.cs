using GoalApi.Dtos.Today;

namespace GoalApi.Services.Interfaces;

public interface ITodayService
{
    Task<TodayDashboardDto> GetTodayItemsAsync(int userId);
}
