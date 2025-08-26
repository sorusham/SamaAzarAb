using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageForAzarab.Models
{
    /// <summary>
    /// این مدل برای نگهداری ارتباط بین کاربران و دپارتمان‌ها استفاده می‌شود
    /// </summary>
    public class UserDepartment
    {
        [Key]
        public int Id { get; set; }
        
        // ارتباط با کاربر
        [Required]
        [Display(Name = "کاربر")]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
        
        // ارتباط با دپارتمان
        [Required]
        [Display(Name = "دپارتمان")]
        public int DepartmentId { get; set; }
        
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; } = null!;
        
        // تاریخ انتساب
        [Display(Name = "تاریخ انتساب")]
        public DateTime AssignmentDate { get; set; } = DateTime.Now;
        
        // نقش کاربر در دپارتمان
        [Display(Name = "نقش")]
        public string? Role { get; set; }
        
        // آیا کاربر مدیر دپارتمان است؟
        [Display(Name = "مدیر دپارتمان")]
        public bool IsDepartmentManager { get; set; } = false;
        
        // توضیحات
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
    }
} 