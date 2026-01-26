using Microsoft.AspNetCore.Identity;

public sealed class FakePasswordHasher : IPasswordHasher<User>
{
    public string HashPassword(User user, string password)
        => "fake-hash";

    public PasswordVerificationResult VerifyHashedPassword(
        User user,
        string hashedPassword,
        string providedPassword)
        => PasswordVerificationResult.Success;
}
