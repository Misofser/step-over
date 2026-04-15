namespace GoalApi.Models;

public class HabitCompletion : IHasTimestamps
{
    public int Id { get; set; }
    public int HabitId  { get; set; }

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Habit Habit { get; set; } = null!;
}
