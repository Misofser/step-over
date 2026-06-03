using GoalApi.Dtos.Habit;

namespace GoalApi.Services.Interfaces;

public interface IHabitService
{
    Task<List<HabitReadDto>> GetHabitsByGoalAsync(int goalId);
    Task<HabitReadDto> GetHabitByIdAsync(int habitId);
    Task<HabitReadDto> AddHabitAsync(int goalId, HabitCreateDto dto);
    Task ToggleCompletion(int habitId, DateTime date);
    Task DeleteHabitAsync(int habitId);
}
