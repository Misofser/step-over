using GoalApi.Dtos.Habit;

namespace GoalApi.Services.Interfaces;

public interface IHabitService
{
    Task<List<HabitReadDto>> GetHabitsByGoalAsync(int userId, int goalId);
    Task<HabitReadDto> GetHabitByIdAsync(int userId, int habitId);
    Task<HabitReadDto> AddHabitAsync(int userId, int goalId, HabitCreateDto dto);
    Task ToggleCompletion(int userId, int habitId, DateTime date);
    Task DeleteHabitAsync(int userId, int habitId);
    Task<HabitCompletionStatusDto> GetCompletionStatusAsync(int userId, int habitId, DateTime date);
}
