using Microsoft.AspNetCore.Identity;

namespace MessageForAzarab.Services.Interface
{
    public interface IRoleService
    {
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task<IdentityRole?> GetRoleByIdAsync(string roleId);
        Task<IdentityRole?> GetRoleByNameAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> UpdateRoleAsync(IdentityRole role);
        Task<IdentityResult> DeleteRoleAsync(string roleId);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
        Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
    }
} 