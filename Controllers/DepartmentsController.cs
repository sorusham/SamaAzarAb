using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using System.Threading.Tasks;
using MessageForAzarab.Models.ViewModels;

namespace MessageForAzarab.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IUserService _userService;

        public DepartmentsController(IDepartmentService departmentService, IUserService userService)
        {
            _departmentService = departmentService;
            _userService = userService;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // دریافت لیست کاربران دپارتمان
            var departmentUsers = await _departmentService.GetDepartmentUsersAsync(id);
            ViewBag.DepartmentUsers = departmentUsers;

            return View(department);
        }

        // GET: Departments/Create
        [Authorize(Roles = "Admin,DCC")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                await _departmentService.CreateDepartmentAsync(department);
                TempData["SuccessMessage"] = "دپارتمان با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _departmentService.UpdateDepartmentAsync(department);
                TempData["SuccessMessage"] = "دپارتمان با موفقیت به‌روزرسانی شد.";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // دریافت لیست کاربران دپارتمان
            var departmentUsers = await _departmentService.GetDepartmentUsersAsync(id);
            ViewBag.DepartmentUsers = departmentUsers;

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            TempData["SuccessMessage"] = "دپارتمان با موفقیت حذف شد.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/ManageUsers/5
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> ManageUsers(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // دریافت تمام کاربران
            var allUsers = await _userService.GetActiveUsersAsync();
            var departmentUsers = await _departmentService.GetDepartmentUsersAsync(id);

            var model = new DepartmentUsersViewModel
            {
                DepartmentId = id,
                DepartmentName = department.Name,
                Users = new List<UserDepartmentViewModel>()
            };

            foreach (var user in allUsers)
            {
                bool isInDepartment = await _departmentService.IsUserInDepartmentAsync(user.Id, id);
                var userDepartment = isInDepartment ? await _departmentService.GetUserDepartmentAsync(user.Id, id) : null;

                model.Users.Add(new UserDepartmentViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    IsSelected = isInDepartment,
                    Role = userDepartment?.Role ?? string.Empty,
                    IsDepartmentManager = userDepartment?.IsDepartmentManager ?? false
                });
            }

            return View(model);
        }

        // POST: Departments/ManageUsers
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,DCC")]
        public async Task<IActionResult> ManageUsers(DepartmentUsersViewModel model)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(model.DepartmentId);
            if (department == null)
            {
                return NotFound();
            }

            foreach (var user in model.Users)
            {
                bool isInDepartment = await _departmentService.IsUserInDepartmentAsync(user.UserId, model.DepartmentId);

                if (user.IsSelected && !isInDepartment)
                {
                    // اضافه کردن کاربر به دپارتمان
                    await _departmentService.AddUserToDepartmentAsync(
                        user.UserId, 
                        model.DepartmentId, 
                        user.Role, 
                        user.IsDepartmentManager);
                }
                else if (!user.IsSelected && isInDepartment)
                {
                    // حذف کاربر از دپارتمان
                    await _departmentService.RemoveUserFromDepartmentAsync(user.UserId, model.DepartmentId);
                }
                else if (user.IsSelected && isInDepartment)
                {
                    // به‌روزرسانی اطلاعات کاربر در دپارتمان
                    var userDepartment = await _departmentService.GetUserDepartmentAsync(user.UserId, model.DepartmentId);
                    if (userDepartment != null)
                    {
                        userDepartment.Role = user.Role;
                        userDepartment.IsDepartmentManager = user.IsDepartmentManager;
                        await _departmentService.UpdateUserDepartmentAsync(userDepartment);
                    }
                }
            }

            TempData["SuccessMessage"] = "کاربران دپارتمان با موفقیت به‌روزرسانی شدند.";
            return RedirectToAction(nameof(Details), new { id = model.DepartmentId });
        }
    }
} 