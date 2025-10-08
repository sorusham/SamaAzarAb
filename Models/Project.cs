using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageForAzarab.Models
{
    // این مدل معادل جدول Ctbproject2 در دیتابیس قدیمی است
    public class Project
    {
        [Key]
        public int Id { get; set; } // معادل SiProject
        
        [Required(ErrorMessage = "کد پروژه الزامی است")]
        [StringLength(50, ErrorMessage = "کد پروژه نمی‌تواند بیش از 50 کاراکتر باشد")]
        [Display(Name = "کد پروژه")]
        public string ProjectCode { get; set; } = string.Empty; // معادل cuproject
        
        [Required(ErrorMessage = "نام پروژه الزامی است")]
        [StringLength(200, ErrorMessage = "نام پروژه نمی‌تواند بیش از 200 کاراکتر باشد")]
        [Display(Name = "نام پروژه")]
        public string Name { get; set; } = string.Empty; // معادل tpproject
        
        [StringLength(1000, ErrorMessage = "شرح پروژه نمی‌تواند بیش از 1000 کاراکتر باشد")]
        [Display(Name = "شرح پروژه")]
        public string? Description { get; set; }
        
        // ارتباط با دپارتمان
        [Display(Name = "دپارتمان")]
        public int? DepartmentId { get; set; }
        
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        
        [Display(Name = "گزینه")]
        public int? OptionId { get; set; } // معادل SiOption
        
        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "تاریخ شروع")]
        public DateTime? StartDate { get; set; }
        
        [Display(Name = "تاریخ پایان")]
        public DateTime? EndDate { get; set; }
        
        // لیست اسناد مرتبط با این پروژه
        public virtual ICollection<BaseDocument> Documents { get; set; } = new List<BaseDocument>();
        
        // لیست تراکنش‌های مرتبط با این پروژه
        public virtual ICollection<DocumentTransaction> Transactions { get; set; } = new List<DocumentTransaction>();
    }
} 