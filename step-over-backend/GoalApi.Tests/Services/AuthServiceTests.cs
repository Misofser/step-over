using GoalApi.Dtos.User;
using GoalApi.Services;

namespace GoalApi.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsUserDto()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, new FakePasswordHasher(true));
        var dto = new LoginDto { Username = "Test User", Password = "Password123" };

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("Test User", result.Username);
        Assert.Equal("User", result.Role);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsAuthenticationException()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new AuthService(db, new FakePasswordHasher(true));

        var dto = new LoginDto { Username = "Test User", Password = "Password123" };

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(
            () => service.LoginAsync(dto)
        );
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsAuthenticationException()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "testhash", Role = "User" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, new FakePasswordHasher(false));

        var dto = new LoginDto{ Username = "Test User", Password = "WrongPassword" };

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(
            () => service.LoginAsync(dto)
        );
    }
}
