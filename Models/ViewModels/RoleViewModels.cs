namespace MessageForAzarab.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class RoleUsersViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<UserRoleViewModel> Users { get; set; } = new List<UserRoleViewModel>();
    }
} 