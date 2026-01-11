using System.ComponentModel.DataAnnotations;

namespace GoalApi.Dtos.User;

public class UserCreateDto
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(1, ErrorMessage = "Username cannot be empty")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}
