using GoalApi.Dtos.User;
using GoalApi.Dtos.Auth;

namespace GoalApi.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task<RefreshResponseDto> RefreshAsync(string refreshToken);
    Task LogoutAsync(string? refreshToken);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
}
