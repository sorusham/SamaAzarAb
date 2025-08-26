using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MessageForAzarab.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public UsersController(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        // لیست کاربران
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // جزئیات کاربر
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // ایجاد کاربر جدید
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user, string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }

        // ویرایش کاربر
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            if (id != user.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _userService.GetUserByIdAsync(id);
                    if (existingUser == null)
                        return NotFound();

                    existingUser.UserName = user.UserName ?? string.Empty;
                    existingUser.Email = user.Email ?? string.Empty;
                    existingUser.FullName = user.FullName ?? string.Empty;
                    existingUser.PhoneNumber = user.PhoneNumber ?? string.Empty;
                    existingUser.PhoneNumber2 = user.PhoneNumber2;
                    existingUser.Address = user.Address ?? string.Empty;
                    existingUser.IsActive = user.IsActive;
                    existingUser.UserType = user.UserType;
                    existingUser.CompanyName = user.CompanyName ?? string.Empty;
                    existingUser.Description = user.Description ?? string.Empty;

                    await _userService.UpdateUserAsync(existingUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _userService.UserExistsAsync(id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // حذف کاربر
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        // فعال/غیرفعال کردن کاربر
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userService.UpdateUserAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        // تغییر رمز عبور کاربر
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, string newPassword)
        {
            if (id == null)
                return NotFound();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError(string.Empty, "رمز عبور را وارد کنید");
                ViewBag.UserId = id;
                ViewBag.UserName = user.UserName;
                return View();
            }

            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, newPassword);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.UserId = id;
            ViewBag.UserName = user.UserName;
            return View();
        }
    }
} 