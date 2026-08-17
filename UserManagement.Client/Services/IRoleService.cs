using UserManagement.Client.Models;

namespace UserManagement.Client.Services;

public interface IRoleService
{
    Task<List<RoleDto>> GetRolesAsync();
}