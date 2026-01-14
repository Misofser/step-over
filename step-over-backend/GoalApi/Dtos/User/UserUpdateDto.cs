using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.User;

/// <summary>
/// Data used to update an existing user.
/// </summary>
public class UserUpdateDto
{
    /// <summary>
    /// New username of the user. Optional, but cannot be empty if provided.
    /// </summary>
    [MinLength(1, ErrorMessage = "Username cannot be empty")]
    public string? Username { get; set; } = null!;
}
