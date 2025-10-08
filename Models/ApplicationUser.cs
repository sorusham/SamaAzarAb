using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MessageForAzarab.Models
{
    public enum UserType
    {
        Expert,         // کارشناس
        Company,        // شرکت
        BusinessMember  // عضو بازرگانی
    }

    public class ApplicationUser : IdentityUser
    {
        // نام کامل کاربر
        [Required(ErrorMessage = "نام کامل الزامی است")]
        [StringLength(100, ErrorMessage = "نام کامل نمی‌تواند بیش از 100 کاراکتر باشد")]
        [Display(Name = "نام کامل")]
        public string FullName { get; set; } = string.Empty;
        
        // فعال یا غیرفعال بودن کاربر
        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;
        
        // نوع کاربر (کارشناس، شرکت، عضو بازرگانی)
        [Display(Name = "نوع کاربر")]
        public UserType UserType { get; set; } = UserType.Expert;
        
        // تاریخ ثبت‌نام
        [Display(Name = "تاریخ ثبت‌نام")]
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        
        // اطلاعات تماس اضافی
        [StringLength(20, ErrorMessage = "شماره تلفن دوم نمی‌تواند بیش از 20 کاراکتر باشد")]
        [Display(Name = "شماره تلفن دوم")]
        public string PhoneNumber2 { get; set; } = string.Empty;
        
        // آدرس
        [StringLength(500, ErrorMessage = "آدرس نمی‌تواند بیش از 500 کاراکتر باشد")]
        [Display(Name = "آدرس")]
        public string Address { get; set; } = string.Empty;
        
        // نام شرکت (اگر نوع کاربر، شرکت باشد)
        [StringLength(200, ErrorMessage = "نام شرکت نمی‌تواند بیش از 200 کاراکتر باشد")]
        [Display(Name = "نام شرکت")]
        public string CompanyName { get; set; } = string.Empty;
        
        // توضیحات
        [StringLength(1000, ErrorMessage = "توضیحات نمی‌تواند بیش از 1000 کاراکتر باشد")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; } = string.Empty;
        
        // نامه‌های ارسال شده
        public virtual ICollection<Letter> SentLetters { get; set; } = new List<Letter>();
        
        // نامه‌های دریافت شده
        public virtual ICollection<Letter> ReceivedLetters { get; set; } = new List<Letter>();
        
        // پیوست‌های ارسال شده
        public virtual ICollection<Attachment> SentAttachments { get; set; } = new List<Attachment>();
        
        // پیوست‌های دریافت شده
        public virtual ICollection<Attachment> ReceivedAttachments { get; set; } = new List<Attachment>();
        
        // بازنگری‌های ارسال شده
        public virtual ICollection<AttachmentRevision> SentRevisions { get; set; } = new List<AttachmentRevision>();
        
        // بازنگری‌های دریافت شده
        public virtual ICollection<AttachmentRevision> ReceivedRevisions { get; set; } = new List<AttachmentRevision>();
        
        // اعلان‌های کاربر
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        
        // دپارتمان‌های کاربر
        public virtual ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
    }
} 