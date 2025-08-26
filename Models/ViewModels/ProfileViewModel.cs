using System.ComponentModel.DataAnnotations;
using MessageForAzarab.Models;

namespace MessageForAzarab.Models.ViewModels
{
    public class ProfileViewModel
    {
        public required string Id { get; set; }

        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [Display(Name = "نام کاربری")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        [Display(Name = "ایمیل")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "لطفا نام کامل خود را وارد کنید")]
        [Display(Name = "نام کامل")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "لطفا شماره تلفن را وارد کنید")]
        [Phone(ErrorMessage = "شماره تلفن وارد شده معتبر نیست")]
        [Display(Name = "شماره تلفن")]
        public required string PhoneNumber { get; set; }

        [Display(Name = "توضیحات")]
        public required string Description { get; set; }

        [Display(Name = "نوع کاربر")]
        public required string UserType { get; set; }
    }
} 