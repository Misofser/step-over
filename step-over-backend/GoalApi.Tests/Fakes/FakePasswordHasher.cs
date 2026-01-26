using Microsoft.AspNetCore.Identity;

public sealed class FakePasswordHasher(bool success = true) : IPasswordHasher<User>
{
    private readonly bool _success = success;

    public string HashPassword(User user, string password)
        => "fake-hash";

    public PasswordVerificationResult VerifyHashedPassword(
        User user,
        string hashedPassword,
        string providedPassword)
        => _success ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
}
