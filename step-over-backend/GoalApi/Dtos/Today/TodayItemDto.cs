using GoalApi.Enums;

namespace GoalApi.Dtos.Today;

/// <summary>Represents an actionable item that should be completed today.</summary>
public class TodayItemDto
{
    /// <summary>Item identifier</summary>
    public int EntityId { get; set; }

    /// <summary>Display title</summary>
    public string Title { get; set; } = null!;

    /// <summary>Item type (Habit or Task)</summary>
    public TodayItemType Type { get; set; }

    /// <summary>Indicates whether the item is completed today</summary>
    public bool IsCompleted { get; set; }

    /// <summary>Identifier of the goal this item belongs to</summary>
    public int GoalId { get; set; }

    /// <summary>Title of the goal this item belongs to</summary>
    public string GoalTitle { get; set; } = null!;
}
