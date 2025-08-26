using System;

namespace MessageForAzarab.Models
{
    public class Notification
    {
        public int Id { get; set; }
        
        // کاربری که اعلان مربوط به او است
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }
        
        // آیا توسط کاربر خوانده شده است؟
        public bool IsRead { get; set; } = false;
        
        // نوع اعلان
        public NotificationType Type { get; set; }
        
        // محتوای اعلان
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        
        // لینک مربوط به اعلان (اگر وجود داشته باشد)
        public string? Link { get; set; }
        
        // زمان ایجاد اعلان
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // زمان خوانده شدن اعلان
        public DateTime? ReadAt { get; set; }
    }
    
    public enum NotificationType
    {
        NewLetter,           // نامه جدید
        NewAttachment,       // پیوست جدید
        NewRevision,         // بازنگری جدید
        LetterExpirySoon,    // نزدیک به انقضای نامه
        SystemMessage,       // پیام سیستمی
        
        // اعلان‌های مربوط به اسناد
        NewDocument,         // سند جدید
        DocumentUpdate,      // بروزرسانی سند
        NewDocumentVersion,  // نسخه جدید سند
        DocumentAssigned,    // تخصیص سند به کاربر
        DocumentApproved,    // تأیید سند
        DocumentRejected     // رد سند
    }
} 