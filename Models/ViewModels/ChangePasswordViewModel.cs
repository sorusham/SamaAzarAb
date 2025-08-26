using System.ComponentModel.DataAnnotations;

namespace MessageForAzarab.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "لطفا رمز عبور فعلی را وارد کنید")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور فعلی")]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "لطفا رمز عبور جدید را وارد کنید")]
        [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} و حداکثر {1} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        public required string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور جدید")]
        [Compare("NewPassword", ErrorMessage = "رمز عبور جدید و تکرار آن مطابقت ندارند")]
        public required string ConfirmPassword { get; set; }
    }
} 