using UserManagement.Client.Models;

namespace UserManagement.Client.Services;

public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync();
    Task CreateUserAsync(CreateUserRequest request);
    Task DeleteUserAsync(int id);
    Task<UserDto?> GetUserAsync(int id);
    Task UpdateUserAsync(int id, UpdateUserRequest request);
}