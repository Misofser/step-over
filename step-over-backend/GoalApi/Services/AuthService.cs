using GoalApi.Dtos.User;
using GoalApi.Dtos.Auth;
using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Exceptions;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GoalApi.Services;

public class AuthService(AppDbContext db, IJwtService jwt, IPasswordHasher<User> passwordHasher) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly IJwtService _jwt = jwt;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null) throw new AuthenticationException();

        if (!IsPasswordValid(user, dto.Password)) throw new AuthenticationException();

        var accessToken = _jwt.GenerateAccessToken(user.Id, user.Username, user.Role);
        var refreshToken = _jwt.GenerateRefreshToken();
        var refreshTokenHash = _jwt.HashRefreshToken(refreshToken);

        _db.RefreshTokens.Add(BuildRefreshToken(user, refreshToken));
        await _db.SaveChangesAsync();

        return new LoginResponseDto
        {
            User = new UserReadDto { Id = user.Id, Username = user.Username, Role = user.Role },
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<RefreshResponseDto> RefreshAsync(string refreshToken)
    {
        var oldHash = _jwt.HashRefreshToken(refreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == oldHash)
            ?? throw new AuthenticationException();
        ValidateRefreshToken(stored);

        var user = await _db.Users.FindAsync(stored.UserId) ?? throw new AuthenticationException();

        var newAccessToken = _jwt.GenerateAccessToken(user.Id, user.Username, user.Role);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        stored.RevokedAt = DateTime.UtcNow;

        _db.RefreshTokens.Add(BuildRefreshToken(user, newRefreshToken));

        await _db.SaveChangesAsync();

        return new RefreshResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return;
        var hash = _jwt.HashRefreshToken(refreshToken);

        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash);
        if (stored == null) return;

        stored.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) throw new NotFoundException("User");

        if (!IsPasswordValid(user, dto.CurrentPassword))
            throw new BadRequestException("Current password is incorrect");
        if (dto.CurrentPassword == dto.NewPassword)
            throw new BadRequestException("New password must be different from the current password");

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        await _db.SaveChangesAsync();
    }

    private RefreshToken BuildRefreshToken(User user, string refreshToken)
    {
        return new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _jwt.HashRefreshToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }

    private static void ValidateRefreshToken(RefreshToken token)
    {
        if (token.ExpiresAt < DateTime.UtcNow) throw new AuthenticationException();
        if (token.RevokedAt != null) throw new AuthenticationException();
    }

    private bool IsPasswordValid(User user, string password)
    {
        return _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password) == PasswordVerificationResult.Success;
    }
}
