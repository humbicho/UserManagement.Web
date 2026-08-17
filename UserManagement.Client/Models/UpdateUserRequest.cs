namespace UserManagement.Client.Models;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Username { get; set; } = string.Empty;

    public string? Password { get; set; }

    public bool IsActive { get; set; }

    public List<int> RoleIds { get; set; } = [];
}