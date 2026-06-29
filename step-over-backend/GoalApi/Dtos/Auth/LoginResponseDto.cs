using GoalApi.Dtos.User;

namespace GoalApi.Dtos.Auth;

public class LoginResponseDto
{
    public UserReadDto User { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
