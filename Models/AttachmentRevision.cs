namespace MessageForAzarab.Models
{
    public class AttachmentRevision
    {
        public int Id { get; set; }

        // شماره نسخه بازنگری (مثلاً 0، 1، 2، ...)
        public int RevisionNumber { get; set; }

        // وضعیت پیوست: تأیید شده، رد شده، نیاز به اصلاح، در حال بررسی و ...
        public string Status { get; set; } = string.Empty;

        // تاریخ ثبت تغییر وضعیت
        public DateTime StatusDate { get; set; }

        // کاربری که وضعیت را تغییر داده
        public string ChangedBy { get; set; } = string.Empty;

        // فرستنده و گیرنده به صورت متنی (برای سازگاری با کد قبلی)
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;

        // فرستنده (ارتباط با کاربر)
        public string SenderId { get; set; } = string.Empty;
        public virtual ApplicationUser? Sender { get; set; }

        // گیرنده (ارتباط با کاربر)
        public string ReceiverId { get; set; } = string.Empty;
        public virtual ApplicationUser? Receiver { get; set; }

        // کامنت یا توضیحات مربوط به تغییر وضعیت
        public string Comment { get; set; } = string.Empty;

        // ارتباط با پیوست (Attachment)
        public int AttachmentId { get; set; }
        public virtual Attachment Attachment { get; set; } = null!;
    }

    public enum AttachmentRevisionStatus
    {
        Commented,
        Approved,
        Rejected,
        approve_as_note
    }
}
