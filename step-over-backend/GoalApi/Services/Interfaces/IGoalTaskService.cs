using GoalApi.Dtos.GoalTask;

namespace GoalApi.Services.Interfaces;

public interface IGoalTaskService
{
    Task<List<GoalTaskReadDto>> GetTasksByGoalAsync(int userId, int goalId);
    Task<GoalTaskReadDto> GetTaskByIdAsync(int userId, int taskId);
    Task<GoalTaskReadDto> AddTaskAsync(int userId, int goalId, GoalTaskCreateDto dto);
    Task UpdateCompletionAsync(int userId, int taskId, GoalTaskUpdateCompletionDto dto);
    Task UpdateTaskAsync(int userId, int taskId, GoalTaskUpdateDto dto);
    Task DeleteTaskAsync(int userId, int taskId);
}
