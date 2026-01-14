namespace GoalApi.Dtos.User;

/// <summary>
/// Represents a user returned by the API.
/// </summary>
public class UserReadDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Username of the user.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Role of the user (e.g., 'User' or 'Admin').
    /// </summary>
    public string Role { get; set; } = "User";
}
