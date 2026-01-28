using GoalApi.Services.Infrastructure;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GoalApi.Tests.Services.Infrastructure;

public class CurrentUserServiceTests
{
    private static CurrentUserService CreateService(IEnumerable<Claim> claims)
        => new CurrentUserService(
            new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            });

    [Fact]
    public void GetUserId_ReturnsId_WhenClaimExists()
    {
        // Arrange
        var service = CreateService(new[] { new Claim(ClaimTypes.NameIdentifier, "1") });

        // Act
        var result = service.GetUserId();

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void GetUserId_Throws_WhenClaimMissing()
    {
        // Arrange
        var service = CreateService(Array.Empty<Claim>());

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => service.GetUserId());
    }

    [Fact]
    public void GetUsername_ReturnsUsername_WhenClaimExists()
    {
        // Arrange
        var service = CreateService(new[] { new Claim(ClaimTypes.Name, "Test User") });

        // Act
        var result = service.GetUsername();

        // Assert
        Assert.Equal("Test User", result);
    }

    [Fact]
    public void GetUsername_Throws_WhenClaimMissing()
    {
        // Arrange
        var service = CreateService(Array.Empty<Claim>());

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => service.GetUsername());
    }

    [Fact]
    public void GetRole_ReturnsRole_WhenClaimExists()
    {
        // Arrange
        var service = CreateService(new[] { new Claim(ClaimTypes.Role, "Admin") });

        // Act
        var result = service.GetRole();

        // Assert
        Assert.Equal("Admin", result);
    }

    [Fact]
    public void GetRole_Throws_WhenClaimMissing()
    {
        // Arrange
        var service = CreateService(Array.Empty<Claim>());

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => service.GetRole());
    }
}
