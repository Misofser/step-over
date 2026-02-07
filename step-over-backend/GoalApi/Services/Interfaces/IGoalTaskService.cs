using GoalApi.Dtos.GoalTask;

namespace GoalApi.Services.Interfaces;

public interface IGoalTaskService
{
    Task<List<GoalTaskReadDto>> GetTasksByGoalAsync(int goalId);
    Task AddTaskAsync(int goalId, GoalTaskCreateDto dto);
    Task UpdateCompletionAsync(int taskId, GoalTaskUpdateCompletionDto dto);
}
