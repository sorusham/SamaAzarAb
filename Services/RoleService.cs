using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MessageForAzarab.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<IdentityRole?> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            return await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        public async Task<IdentityResult> UpdateRoleAsync(IdentityRole role)
        {
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                return await _roleManager.DeleteAsync(role);
            }
            return IdentityResult.Failed(new IdentityError { Description = "نقش پیدا نشد" });
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return await _userManager.GetRolesAsync(user);
            }
            return new List<string>();
        }

        public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return await _userManager.AddToRoleAsync(user, roleName);
            }
            return IdentityResult.Failed(new IdentityError { Description = "کاربر پیدا نشد" });
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            return IdentityResult.Failed(new IdentityError { Description = "کاربر پیدا نشد" });
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return await _userManager.IsInRoleAsync(user, roleName);
            }
            return false;
        }
    }
} 