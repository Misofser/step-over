using GoalApi.Dtos.Goal;

namespace GoalApi.Services.Interfaces;

public interface IGoalService
{
    Task<List<GoalReadDto>> GetAllGoalsAsync(int userId);
    Task<GoalReadDto> GetGoalByIdAsync(int userId, int id);
    Task<GoalReadDto> CreateGoalAsync(int userId, GoalCreateDto dto);
    Task UpdateGoalAsync(int userId, int goalId, GoalUpdateDto dto);
    Task DeleteGoalAsync(int userId, int goalId);
}
