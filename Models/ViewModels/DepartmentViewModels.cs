namespace MessageForAzarab.Models.ViewModels
{
    public class UserDepartmentViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public string? Role { get; set; }
        public bool IsDepartmentManager { get; set; }
    }

    public class DepartmentUsersViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public List<UserDepartmentViewModel> Users { get; set; } = new List<UserDepartmentViewModel>();
    }
} 