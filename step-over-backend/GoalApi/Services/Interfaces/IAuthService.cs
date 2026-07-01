using GoalApi.Dtos.User;
using GoalApi.Dtos.Auth;

namespace GoalApi.Services.Interfaces;

public interface IAuthService
{
    Task<UserReadDto> LoginAsync(LoginDto dto);
}
