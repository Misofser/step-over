using GoalApi.Dtos.User;

namespace GoalApi.Services.Interfaces;

public interface IAuthService
{
    Task<UserReadDto> LoginAsync(LoginDto dto);
}
