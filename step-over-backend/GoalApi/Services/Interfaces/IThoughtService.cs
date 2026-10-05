using GoalApi.Dtos.Thought;

namespace GoalApi.Services.Interfaces;

public interface IThoughtService
{
    Task<ThoughtReadDto> CreateThoughtAsync(int userId, ThoughtCreateDto dto);
}
