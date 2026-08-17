namespace UserManagement.Client.Models;
using System.ComponentModel.DataAnnotations;

public class CreateUserRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Range(18, 100)]
    public int Age { get; set; }

    [Required]
    [MinLength(5)]
    [RegularExpression(@"^\S+$",
    ErrorMessage = "Username cannot contain spaces.")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
    ErrorMessage = "Password must contain 8 characters, one uppercase, one lowercase and one number.")]
    public string Password { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public List<int> RoleIds { get; set; } = [];
    public bool UseUsernameAsDisplayName { get; set; }
}