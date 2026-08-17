using System.Net.Http.Json;
using UserManagement.Client.Models;

namespace UserManagement.Client.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>(
            "api/users")
            ?? [];
    }

    public async Task CreateUserAsync(CreateUserRequest request)
    {
        await _httpClient.PostAsJsonAsync(
            "api/users",
            request);
    }
    public async Task DeleteUserAsync(int id)
    {
        await _httpClient.DeleteAsync(
            $"api/users/{id}");
    }
    public async Task<UserDto?> GetUserAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>(
            $"api/users/{id}");
    }
    public async Task UpdateUserAsync(int id, UpdateUserRequest request)
    {
        await _httpClient.PutAsJsonAsync(
            $"api/users/{id}",
            request);
    }
}