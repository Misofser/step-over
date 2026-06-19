namespace GoalApi.Dtos.Today;

/// <summary>
/// Dashboard representation of today's user activity,
/// grouped into pending and completed items.
/// </summary>
public class TodayDashboardDto
{
    /// <summary>Actions that still need to be completed today</summary>
    public List<TodayItemDto> Pending { get; set; } = new();

    /// <summary>Actions completed during the current day</summary>
    public List<TodayItemDto> Completed { get; set; } = new();
}
