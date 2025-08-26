using MessageForAzarab.Models;

namespace MessageForAzarab.Services.Interface
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<Department> CreateDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(int id);
        
        // متدهای مرتبط با کاربران دپارتمان
        Task<List<ApplicationUser>> GetDepartmentUsersAsync(int departmentId);
        Task AddUserToDepartmentAsync(string userId, int departmentId, string? role = null, bool isDepartmentManager = false);
        Task RemoveUserFromDepartmentAsync(string userId, int departmentId);
        Task<List<Department>> GetUserDepartmentsAsync(string userId);
        Task<bool> IsUserInDepartmentAsync(string userId, int departmentId);
        Task<UserDepartment?> GetUserDepartmentAsync(string userId, int departmentId);
        Task UpdateUserDepartmentAsync(UserDepartment userDepartment);
    }
} 