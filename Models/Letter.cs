using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageForAzarab.Models
{
    public class Letter
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "کد")]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "تاریخ شروع")]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Display(Name = "تاریخ انقضا")]
        public DateTime ExpiryDate { get; set; }
        
        [Required]
        [Display(Name = "از")]
        public string From { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "به")]
        public string To { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "عنوان")]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "توضیحات")]
        public string Description { get; set; } = string.Empty;
        
        // ارتباط با فرستنده (کاربر ارسال کننده)
        public string? SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual ApplicationUser? Sender { get; set; }
        
        // ارتباط با گیرنده (کاربر دریافت کننده)
        public string? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser? Receiver { get; set; }
        
        // ارتباط با پیوست‌ها
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
} 