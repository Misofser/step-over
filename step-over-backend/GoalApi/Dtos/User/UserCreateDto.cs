using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.User;

/// <summary>
/// Data required to create a new user.
/// </summary>
public class UserCreateDto
{
    /// <summary>
    /// Username of the new user.
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = null!;

    /// <summary>
    /// Password of the new user.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(10, ErrorMessage = "Password must be at least 10 characters long")]
    public string Password { get; set; } = null!;
}
