using GoalApi.Services.Infrastructure;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace GoalApi.Tests.Services.Infrastructure;

public class JwtServiceTests
{
    private static JwtService CreateService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "super-secret-test-key-123456789-123456789"
            })
            .Build();

        return new JwtService(config);
    }

    [Fact]
    public void GenerateToken_ReturnsNonEmptyToken()
    {
        // Arrange
        var service = CreateService();

        // Act
        var token = service.GenerateToken(1, "Test User", "Admin");

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateToken_ContainsCorrectClaims()
    {
        // Arrange
        var service = CreateService();
        var handler = new JwtSecurityTokenHandler();

        // Act
        var tokenString = service.GenerateToken(1, "Test User", "User");
        var token = handler.ReadJwtToken(tokenString);

        // Assert
        Assert.Equal("1", token.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal("Test User", token.Claims.Single(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal("User", token.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
    }

    [Fact]
    public void GenerateToken_CanBeValidatedWithSameSecret()
    {
        // Arrange
        var service = CreateService();
        var secret = "super-secret-test-key-123456789-123456789"; // same test secret

        var token = service.GenerateToken(1, "Test User", "Admin");

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secret)
            ),
            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();

        // Act
        var principal = handler.ValidateToken(token, validationParameters, out _);

        // Assert
        Assert.NotNull(principal);
        Assert.Equal("1", principal.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal("Test User", principal.FindFirst(ClaimTypes.Name)!.Value);
        Assert.Equal("Admin", principal.FindFirst(ClaimTypes.Role)!.Value);
    }
}
