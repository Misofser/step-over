using GoalApi.Services.Infrastructure.Interfaces;

namespace GoalApi.Tests.Fakes;

public sealed class FakeJwtService : IJwtService
{
    public string GenerateAccessToken(int userId, string username, string role)
        => "fake-access-token";

    public string GenerateRefreshToken()
        => "fake-refresh-token";

    public string HashRefreshToken(string refreshToken)
        => $"hash-{refreshToken}";
}
