using GoalApi.Enums;

namespace GoalApi.Models;

public class Habit : IHasTimestamps
{
    public int Id { get; set; }
    public int GoalId { get; set; }

    public string Title { get; set; } = null!;

    public HabitFrequency Frequency { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Goal Goal { get; set; } = null!;

    public List<HabitCompletion> Completions { get; set; } = new List<HabitCompletion>();
}
