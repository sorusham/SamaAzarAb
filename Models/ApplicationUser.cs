using Microsoft.AspNetCore.Identity;

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
        public string FullName { get; set; } = string.Empty;
        
        // فعال یا غیرفعال بودن کاربر
        public bool IsActive { get; set; } = true;
        
        // نوع کاربر (کارشناس، شرکت، عضو بازرگانی)
        public UserType UserType { get; set; } = UserType.Expert;
        
        // تاریخ ثبت‌نام
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        
        // اطلاعات تماس اضافی
        public string PhoneNumber2 { get; set; } = string.Empty;
        
        // آدرس
        public string Address { get; set; } = string.Empty;
        
        // نام شرکت (اگر نوع کاربر، شرکت باشد)
        public string CompanyName { get; set; } = string.Empty;
        
        // توضیحات
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