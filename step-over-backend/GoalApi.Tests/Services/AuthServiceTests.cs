using GoalApi.Dtos.Auth;
using GoalApi.Services;

namespace GoalApi.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponseDto()
    {
        // Arrange
        var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "hash:Password123" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());
        var dto = new LoginDto { Username = "Test User", Password = "Password123" };

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(user.Id, result.User.Id);
            Assert.Equal("Test User", result.User.Username);
            Assert.Equal("User", result.User.Role);
            Assert.Equal("fake-access-token", result.AccessToken);
            Assert.Equal("fake-refresh-token", result.RefreshToken);
        });

        var storedToken = await db.RefreshTokens.SingleAsync();

        Assert.Multiple(() =>
        {
            Assert.Equal(user.Id, storedToken.UserId);
            Assert.Equal("hash-fake-refresh-token", storedToken.TokenHash);
            Assert.Null(storedToken.RevokedAt);
        });
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsAuthenticationException()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());

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

        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());

        var dto = new LoginDto{ Username = "Test User", Password = "WrongPassword" };

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(
            () => service.LoginAsync(dto)
        );
    }

    [Fact]
    public async Task RefreshAsync_ValidToken_ReturnsNewTokensAndRotates()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var jwt = new FakeJwtService();
        var user = new User { Username = "Test User", PasswordHash = "testhash", Role = "User" };
        var oldRefreshToken = "old-token";
        var oldToken = new RefreshToken
        {
            User = user,
            TokenHash = jwt.HashRefreshToken(oldRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        db.RefreshTokens.Add(oldToken);
        await db.SaveChangesAsync();
        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());

        // Act
        var result = await service.RefreshAsync(oldRefreshToken);

        // Assert
        Assert.Equal("fake-access-token", result.AccessToken);
        Assert.Equal("fake-refresh-token", result.RefreshToken);

        Assert.NotNull(oldToken.RevokedAt);

        Assert.Equal(2, db.RefreshTokens.Count());

        var newToken = await db.RefreshTokens
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.Equal(user.Id, newToken!.UserId);
            Assert.Equal("hash-fake-refresh-token", newToken!.TokenHash);
            Assert.Null(newToken!.RevokedAt);
        });
    }

    [Fact]
    public async Task RefreshAsync_InvalidToken_ThrowsAuthenticationException()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() =>
            service.RefreshAsync("non-existing-token"));
    }

    [Fact]
    public async Task RefreshAsync_ExpiredToken_ThrowsAuthenticationException()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var refreshToken = "old-token";
        var jwt = new FakeJwtService();

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.RefreshTokens.Add(new RefreshToken {
            User = user,
            TokenHash = jwt.HashRefreshToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        });
        await db.SaveChangesAsync();
        var service = new AuthService(db, jwt, new FakePasswordHasher());

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() =>
            service.RefreshAsync(refreshToken));
    }

    [Fact]
    public async Task RefreshAsync_RevokedToken_ThrowsAuthenticationException()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var jwt = new FakeJwtService();
        var refreshToken = "old-token";
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.RefreshTokens.Add(new RefreshToken
        {
            User = user,
            TokenHash = jwt.HashRefreshToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            RevokedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        var service = new AuthService(db, jwt, new FakePasswordHasher());

        // Act & Assert
        await Assert.ThrowsAsync<AuthenticationException>(() =>
            service.RefreshAsync(refreshToken));
    }

    [Fact]
    public async Task LogoutAsync_WhenTokenExists_RevokesToken()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var jwt = new FakeJwtService();
        var token = "refresh-token";
        var hash = jwt.HashRefreshToken(token);
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        var stored = new RefreshToken { TokenHash = hash, User = user, ExpiresAt = DateTime.UtcNow.AddDays(1) };
        db.RefreshTokens.Add(stored);
        await db.SaveChangesAsync();
        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());

        // Act
        await service.LogoutAsync(token);

        // Assert
        var result = db.RefreshTokens.Single();

        Assert.NotNull(result.RevokedAt);
        Assert.True(result.RevokedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task LogoutAsync_WhenTokenIsNull_DoesNothing()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());

        // Act
        await service.LogoutAsync(null);

        // Assert
        Assert.Equal(0, db.RefreshTokens.Count());
    }

    [Fact]
    public async Task LogoutAsync_WhenTokenNotFound_DoesNothing()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.RefreshTokens.Add(new RefreshToken { TokenHash = "other-hash", User = user, ExpiresAt = DateTime.UtcNow.AddDays(1) });
        await db.SaveChangesAsync();
        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());

        var token = "real-refresh-token";

        // Act
        await service.LogoutAsync(token);

        // Assert
        var stored = db.RefreshTokens.Single();
        Assert.Null(stored.RevokedAt);
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidPasswords_ChangesPasswordHash()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var passwordHasher = new FakePasswordHasher();
        var user = new User { Username = "Test User", PasswordHash = "hash:OldPassword123" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());
        var dto = new ChangePasswordDto { CurrentPassword = "OldPassword123", NewPassword = "NewPassword123" };

        // Act
        await service.ChangePasswordAsync(user.Id, dto);

        // Assert
        var userInDb = await db.Users.FindAsync(user.Id);
        Assert.NotNull(userInDb);
        Assert.Equal("hash:NewPassword123", userInDb.PasswordHash);
    }

    [Fact]
    public async Task ChangePasswordAsync_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());
        var dto = new ChangePasswordDto { CurrentPassword = "OldPassword123", NewPassword = "NewPassword123" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.ChangePasswordAsync(1, dto));
    }

    [Fact]
    public async Task ChangePasswordAsync_IncorrectCurrentPassword_ThrowsBadRequestException()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "hash:OldPassword123" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());
        var dto = new ChangePasswordDto { CurrentPassword = "IncorrectPassword", NewPassword = "NewPassword123" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BadRequestException>(
            () => service.ChangePasswordAsync(user.Id, dto));

        Assert.Equal("Current password is incorrect", ex.Message);
    }

    [Fact]
    public async Task ChangePasswordAsync_SamePassword_ThrowsBadRequestException()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var user = new User { Username = "Test User", PasswordHash = "hash:Password123" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new AuthService(db, new FakeJwtService(), new FakePasswordHasher());
        var dto = new ChangePasswordDto { CurrentPassword = "Password123", NewPassword = "Password123" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<BadRequestException>(
            () => service.ChangePasswordAsync(user.Id, dto));

        Assert.Equal("New password must be different from the current password", ex.Message);
    }
}
