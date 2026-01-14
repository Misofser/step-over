namespace GoalApi.Services.Infrastructure.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string username, string role);
}
