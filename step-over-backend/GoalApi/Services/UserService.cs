using GoalApi.Data;
using GoalApi.Dtos.User;
using GoalApi.Models;
using GoalApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class UserService(AppDbContext db, IPasswordHasher<User> passwordHasher) : IUserService
{
    private readonly AppDbContext _db = db;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<List<UserReadDto>> GetAllUsersAsync()
    {
        return await _db.Users
            .Select(user => new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
            })
            .ToListAsync();
    }

    public async Task<UserReadDto> GetUserByIdAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) throw new NotFoundException("User");

        return new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
        {
            throw new ConflictException("User with this username already exists");
        }

        var user = new User
        {
            Username = dto.Username,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task UpdateUserAsync(int id, UserUpdateDto updatedUser)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) throw new NotFoundException("User");

        if (!string.IsNullOrWhiteSpace(updatedUser.Username))
        {
            bool usernameExists = await _db.Users.AnyAsync(u => u.Username == updatedUser.Username && u.Id != id);

            if (usernameExists) throw new ConflictException("User with this username already exists");
            user.Username = updatedUser.Username.Trim();
        }

        await _db.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) throw new NotFoundException("User");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }
}
