namespace GoalApi.Dtos;

public class GoalReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
}
