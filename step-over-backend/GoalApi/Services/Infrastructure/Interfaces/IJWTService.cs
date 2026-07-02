namespace GoalApi.Services.Infrastructure.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(int userId, string username, string role);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
