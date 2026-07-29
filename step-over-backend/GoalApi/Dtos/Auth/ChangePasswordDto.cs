using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.Auth;

/// <summary>
/// Represents a request to change the password of the currently authenticated user.
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// The user's current password.
    /// </summary>
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = null!;

    /// <summary>
    /// The new password that will replace the current one.
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [MinLength(10, ErrorMessage = "Password must be at least 10 characters long")]
    public string NewPassword { get; set; } = null!;
}
