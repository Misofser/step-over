using GoalApi.Dtos.User;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GoalApi.Services;

public class AuthService(AppDbContext db, IPasswordHasher<User> passwordHasher, ICurrentUserService currentUser) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<UserReadDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null) throw new AuthenticationException();

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password
        );

        if (result == PasswordVerificationResult.Failed) throw new AuthenticationException();

        return new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
    }

    public UserReadDto GetCurrentUser()
    {
        return new UserReadDto
        {
            Id = _currentUser.GetUserId(),
            Username = _currentUser.GetUsername(),
            Role = _currentUser.GetRole()
        };
    }
}
