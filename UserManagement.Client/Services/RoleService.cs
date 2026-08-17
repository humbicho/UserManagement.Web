using System.Net.Http.Json;
using UserManagement.Client.Models;

namespace UserManagement.Client.Services;

public class RoleService : IRoleService
{
    private readonly HttpClient _httpClient;

    public RoleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<RoleDto>>(
            "api/roles")
            ?? [];
    }
}