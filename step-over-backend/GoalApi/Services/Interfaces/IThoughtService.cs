using GoalApi.Dtos.Thought;

namespace GoalApi.Services.Interfaces;

public interface IThoughtService
{
    Task<List<ThoughtReadDto>> GetAllThoughtsAsync(int userId);
    Task<ThoughtReadDto> CreateThoughtAsync(int userId, ThoughtCreateDto dto);
}
