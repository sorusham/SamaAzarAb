using MessageForAzarab.Models;

namespace MessageForAzarab.Services.Interface
{
    public interface IAttachmentRevisionService
    {
        // دریافت همه بازنگری‌های یک پیوست
        Task<List<AttachmentRevision>> GetRevisionsByAttachmentIdAsync(int attachmentId);
        
        // دریافت یک بازنگری با شناسه
        Task<AttachmentRevision?> GetRevisionByIdAsync(int id);
        
        // ایجاد بازنگری جدید
        Task<AttachmentRevision> CreateRevisionAsync(int attachmentId, string status, string comment, string changedBy);
        
        // به‌روزرسانی وضعیت بازنگری
        Task<AttachmentRevision> UpdateRevisionStatusAsync(int revisionId, string newStatus, string comment, string changedBy);
        
        // حذف بازنگری
        Task DeleteRevisionAsync(int id);
    }
} 