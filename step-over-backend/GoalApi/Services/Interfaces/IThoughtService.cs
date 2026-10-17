using GoalApi.Dtos.Thought;

namespace GoalApi.Services.Interfaces;

public interface IThoughtService
{
    Task<List<ThoughtReadDto>> GetAllThoughtsAsync(int userId);
    Task<ThoughtReadDto> GetThoughtByIdAsync(int userId, int thoughtId);
    Task<ThoughtReadDto> CreateThoughtAsync(int userId, ThoughtCreateDto dto);
    Task UpdateThoughtAsync(int userId, int thoughtId, ThoughtUpdateDto dto);
}
