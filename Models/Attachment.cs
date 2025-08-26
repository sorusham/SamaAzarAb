using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageForAzarab.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "عنوان")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "توضیحات")]
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "تاریخ ارسال")]
        public DateTime DateSent { get; set; } = DateTime.Now;
        
        // فرستنده و گیرنده به صورت متنی (برای سازگاری با کد قبلی)
        [Required]
        [Display(Name = "از")]
        public string From { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "به")]
        public string To { get; set; } = string.Empty;
        
        // فرستنده (ارتباط با کاربر)
        public string? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual ApplicationUser? Sender { get; set; }
        
        // گیرنده (ارتباط با کاربر)
        public string? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser? Receiver { get; set; }
        
        // ارتباط با نامه
        [Required]
        public int LetterId { get; set; }
        [ForeignKey("LetterId")]
        public virtual Letter Letter { get; set; } = null!;
        
        // لیست فایل‌های پیوست
        public virtual ICollection<AttachmentFile> Files { get; set; } = new List<AttachmentFile>();
        
        // لیست بازنگری‌های پیوست
        public virtual ICollection<AttachmentRevision> Revisions { get; set; } = new List<AttachmentRevision>();
    }
} 