using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.User;

public class UserUpdateDto
{
    [MinLength(1, ErrorMessage = "Username cannot be empty")]
    public string? Username { get; set; } = null!;
}
