using GoalApi.Dtos.User;

namespace GoalApi.Services.Interfaces;

public interface IUserService
{
    Task<List<UserReadDto>> GetAllUsersAsync();
    Task<UserReadDto> GetUserByIdAsync(int id);
    Task<UserReadDto> CreateUserAsync(UserCreateDto dto);
    Task UpdateUserAsync(int id, UserUpdateDto updatedUser);
    Task DeleteUserAsync(int id);
}
