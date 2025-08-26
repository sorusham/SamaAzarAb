using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using MessageForAzarab.Models.ViewModels;

namespace MessageForAzarab.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public RolesController(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService;
        }

        // لیست نقش‌ها
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }

        // ایجاد نقش جدید
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("", "نام نقش الزامی است");
                return View();
            }

            var roleExists = await _roleService.RoleExistsAsync(roleName);
            if (roleExists)
            {
                ModelState.AddModelError("", "این نقش قبلاً ایجاد شده است");
                return View();
            }

            await _roleService.CreateRoleAsync(roleName);
            return RedirectToAction(nameof(Index));
        }

        // ویرایش نقش
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string roleName)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();

            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("", "نام نقش الزامی است");
                return View(role);
            }

            role.Name = roleName;
            await _roleService.UpdateRoleAsync(role);
            return RedirectToAction(nameof(Index));
        }

        // حذف نقش
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _roleService.DeleteRoleAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // مدیریت کاربران نقش
        [HttpGet]
        public async Task<IActionResult> ManageUsers(string roleId)
        {
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null)
                return NotFound();

            var model = new RoleUsersViewModel
            {
                RoleId = roleId,
                RoleName = role.Name ?? string.Empty
            };

            // دریافت لیست کاربران
            var allUsers = await _userService.GetAllUsersAsync();
            
            foreach (var user in allUsers)
            {
                bool isInRole = await _roleService.IsUserInRoleAsync(user.Id, role.Name ?? string.Empty);
                model.Users.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    IsSelected = isInRole
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUsers(RoleUsersViewModel model)
        {
            var role = await _roleService.GetRoleByIdAsync(model.RoleId);
            if (role == null)
                return NotFound();

            foreach (var user in model.Users)
            {
                var roleName = role.Name ?? string.Empty;
                var isInRole = await _roleService.IsUserInRoleAsync(user.UserId, roleName);
                
                if (user.IsSelected && !isInRole)
                {
                    await _roleService.AddUserToRoleAsync(user.UserId, roleName);
                }
                else if (!user.IsSelected && isInRole)
                {
                    await _roleService.RemoveUserFromRoleAsync(user.UserId, roleName);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 