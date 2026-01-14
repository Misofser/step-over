using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.User;

/// <summary>
/// Data required for user login.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Username of the user.
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = null!;
    /// <summary>
    /// Password of the user.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}
