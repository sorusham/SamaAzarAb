using MessageForAzarab.Data;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MessageForAzarab.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        
        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.OrderBy(d => d.Name).ToListAsync();
        }
        
        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Documents)
                .Include(d => d.UserDepartments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            if (department.CreatedAt == null)
                department.CreatedAt = DateTime.Now;
                
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }
        
        public async Task UpdateDepartmentAsync(Department department)
        {
            var existingDepartment = await _context.Departments.FindAsync(department.Id);
            if (existingDepartment != null)
            {
                _context.Entry(existingDepartment).CurrentValues.SetValues(department);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }
        
        // متدهای مرتبط با کاربران دپارتمان
        public async Task<List<ApplicationUser>> GetDepartmentUsersAsync(int departmentId)
        {
            return await _context.UserDepartments
                .Where(ud => ud.DepartmentId == departmentId)
                .Include(ud => ud.User)
                .Select(ud => ud.User)
                .ToListAsync();
        }
        
        public async Task AddUserToDepartmentAsync(string userId, int departmentId, string? role = null, bool isDepartmentManager = false)
        {
            // بررسی اینکه آیا کاربر در حال حاضر در دپارتمان هست یا خیر
            var existingUserDepartment = await _context.UserDepartments
                .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DepartmentId == departmentId);
                
            if (existingUserDepartment == null)
            {
                var userDepartment = new UserDepartment
                {
                    UserId = userId,
                    DepartmentId = departmentId,
                    Role = role,
                    IsDepartmentManager = isDepartmentManager,
                    AssignmentDate = DateTime.Now
                };
                
                _context.UserDepartments.Add(userDepartment);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task RemoveUserFromDepartmentAsync(string userId, int departmentId)
        {
            var userDepartment = await _context.UserDepartments
                .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DepartmentId == departmentId);
                
            if (userDepartment != null)
            {
                _context.UserDepartments.Remove(userDepartment);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<List<Department>> GetUserDepartmentsAsync(string userId)
        {
            return await _context.UserDepartments
                .Where(ud => ud.UserId == userId)
                .Include(ud => ud.Department)
                .Select(ud => ud.Department)
                .ToListAsync();
        }
        
        public async Task<bool> IsUserInDepartmentAsync(string userId, int departmentId)
        {
            return await _context.UserDepartments
                .AnyAsync(ud => ud.UserId == userId && ud.DepartmentId == departmentId);
        }
        
        public async Task<UserDepartment?> GetUserDepartmentAsync(string userId, int departmentId)
        {
            return await _context.UserDepartments
                .FirstOrDefaultAsync(ud => ud.UserId == userId && ud.DepartmentId == departmentId);
        }
        
        public async Task UpdateUserDepartmentAsync(UserDepartment userDepartment)
        {
            var existingUserDepartment = await _context.UserDepartments.FindAsync(userDepartment.Id);
            if (existingUserDepartment != null)
            {
                _context.Entry(existingUserDepartment).CurrentValues.SetValues(userDepartment);
                await _context.SaveChangesAsync();
            }
        }
    }
} 