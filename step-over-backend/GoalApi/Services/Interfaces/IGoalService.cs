using GoalApi.Dtos.Goal;

public interface IGoalService
{
    Task<List<GoalReadDto>> GetAllGoalsAsync();
    Task<GoalReadDto> GetGoalByIdAsync(int id);
    Task<GoalReadDto> CreateGoalAsync(int userId, GoalCreateDto dto);
    Task UpdateGoalAsync(int goalId, GoalUpdateDto dto);
    Task DeleteGoalAsync(int id);
}
