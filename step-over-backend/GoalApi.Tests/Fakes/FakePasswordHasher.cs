using Microsoft.AspNetCore.Identity;

namespace GoalApi.Tests.Fakes;

public sealed class FakePasswordHasher : IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
        => $"hash:{password}";

    public PasswordVerificationResult VerifyHashedPassword(
        User user,
        string hashedPassword,
        string providedPassword)
        => hashedPassword == $"hash:{providedPassword}"
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
}
