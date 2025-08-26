using System.ComponentModel.DataAnnotations;

namespace MessageForAzarab.Models
{
    public class AttachmentCreateViewModel
    {
        [Required(ErrorMessage = "عنوان الزامی است")]
        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int LetterId { get; set; }

        //[ValidateFile(MaxSize = 5 * 1024 * 1024)] // مثال برای اعتبارسنجی سفارشی
        public List<IFormFile>? Files { get; set; }
    }
}
