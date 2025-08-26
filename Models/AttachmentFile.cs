namespace MessageForAzarab.Models
{
    public class AttachmentFile
    {
        public int Id { get; set; }

        // نام اصلی فایل
        public string OriginalFileName { get; set; } = string.Empty;

        // نام فایل در سیستم
        public string StoredFileName { get; set; } = string.Empty;

        // نوع فایل
        public string FileType { get; set; } = string.Empty;

        // حجم فایل
        public long FileSize { get; set; }

        // مسیر ذخیره‌سازی فایل
        public string FilePath { get; set; } = string.Empty;

        // تاریخ آپلود فایل
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // ارتباط با پیوست
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; } = default!;
    }
}
