using GoalApi.Dtos.User;
using GoalApi.Services;

namespace GoalApi.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        var user1 = new User { Username = "User 1", PasswordHash = "testhash", Role = "User" };
        var user2 = new User { Username = "User 2", PasswordHash = "testhash", Role = "Admin" };
        db.Users.AddRange(user1, user2);
        await db.SaveChangesAsync();

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(result, u => u.Username == "User 1" && u.Role == "User");
        Assert.Contains(result, u => u.Username == "User 2" && u.Role == "Admin");
    }

    [Fact]
    public async Task GetAllUsersAsync_DoesNotExposePassword()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        var dto = result.Single();

        var propertyNames = dto.GetType()
            .GetProperties()
            .Select(p => p.Name);

        Assert.DoesNotContain(
            propertyNames,
            name => name.Contains("Password"));
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        var user1 = new User { Username = "User 1", PasswordHash = "testhash", Role = "User" };
        var user2 = new User { Username = "User 2", PasswordHash = "testhash", Role = "Admin" };
        db.Users.AddRange(user1, user2);
        await db.SaveChangesAsync();

        // Act
        var result = await service.GetUserByIdAsync(user2.Id);

        // Assert
        Assert.Equal(user2.Id, result.Id);
        Assert.Equal("User 2", result.Username);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task GetUserByIdAsync_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetUserByIdAsync(100));
    }

    [Fact]
    public async Task CreateUserAsync_CreatesNewUser_WhenUsernameIsUnique()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        var dto = new UserCreateDto { Username = "Test User", Password = "Password123" };

        // Act
        var result = await service.CreateUserAsync(dto);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("Test User", result.Username);
        Assert.Equal("User", result.Role);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsConflictException_WhenUsernameAlreadyExists()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        var user = new User { Username = "Test User", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var dto = new UserCreateDto { Username = "Test User", Password = "Password123" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ConflictException>(
            () => service.CreateUserAsync(dto));

        Assert.Equal("User with this username already exists", ex.Message);
    }

    [Fact]
    public async Task CreateUserAsync_HashesPasswordUsingPasswordHasher()
    {
        // Arrange
        var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        var dto = new UserCreateDto { Username = "Test User", Password = "Password123" };

        // Act
        var result = await service.CreateUserAsync(dto);

        // Assert
        var userInDb = await db.Users.FindAsync(result.Id);
        Assert.NotNull(userInDb);
        Assert.Equal("fake-hash", userInDb.PasswordHash);
    }

    [Fact]
    public async Task UpdateUserAsync_UserNotFound_ThrowsNotFound()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var service = new UserService(db, new FakePasswordHasher());

        var dto = new UserUpdateDto { Username = "newname" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateUserAsync(100, dto));
    }

    [Fact]
    public async Task UpdateUserAsync_ValidUsername_UpdatesUsername()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var user = new User { Username = "oldname", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db, new FakePasswordHasher());
        var dto = new UserUpdateDto { Username = " newname " };

        // Act
        await service.UpdateUserAsync(user.Id, dto);

        // Assert
        var userInDb = await db.Users.FindAsync(user.Id);

        Assert.NotNull(userInDb);
        Assert.Equal("newname", userInDb.Username);
    }

    [Fact]
    public async Task UpdateUserAsync_UsernameAlreadyExists_ThrowsConflict()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var user1 = new User { Username = "User 1", PasswordHash = "testhash" };
        var user2 = new User { Username = "User 2", PasswordHash = "testhash" };
        db.Users.AddRange(user1, user2);
        await db.SaveChangesAsync();

        var service = new UserService(db, new FakePasswordHasher());
        var dto = new UserUpdateDto { Username = "User 2" };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(
            () => service.UpdateUserAsync(user1.Id, dto));
    }

    [Fact]
    public async Task UpdateUserAsync_EmptyUsername_DoesNotChangeUsername()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var user = new User { Username = "original", PasswordHash = "testhash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db, new FakePasswordHasher());
        var dto = new UserUpdateDto { Username = "   " };

        // Act
        await service.UpdateUserAsync(user.Id, dto);

        // Assert
        var userInDb = await db.Users.FindAsync(user.Id);

        Assert.NotNull(userInDb);
        Assert.Equal("original", userInDb.Username);
    }

    [Fact]
    public async Task DeleteUserAsync_ValidUser_DeletesUser()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var currentUser = new User { Username = "currentUser", PasswordHash = "testhash", Role = "Admin" };
        var userToDelete = new User { Username = "userToDelete", PasswordHash = "testhash", Role = "User" };
        db.Users.AddRange(currentUser, userToDelete);
        await db.SaveChangesAsync();

        var service = new UserService(db, new FakePasswordHasher());

        // Act
        await service.DeleteUserAsync(currentUser.Id, userToDelete.Id);

        // Assert
        var deletedUser = await db.Users.FindAsync(userToDelete.Id);
        Assert.Null(deletedUser);

        var remainingUsersCount = await db.Users.CountAsync();
        Assert.Equal(1, remainingUsersCount);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenDeletingSelf_ThrowsBadRequest()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var user = new User { Username = "Test User", PasswordHash = "hash", Role = "Admin" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db, new FakePasswordHasher());

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => service.DeleteUserAsync(user.Id, user.Id));
    }

    [Fact]
    public async Task DeleteUserAsync_UserNotFound_ThrowsNotFound()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        var service = new UserService(db, new FakePasswordHasher());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.DeleteUserAsync(currentUserId: 1, id: 100));
    }
}
