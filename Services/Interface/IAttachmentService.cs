using MessageForAzarab.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MessageForAzarab.Services.Interface
{
    public interface IAttachmentService
    {
        // دریافت همه پیوست‌های یک نامه
        Task<List<Attachment>> GetAttachmentsByLetterIdAsync(int letterId);
        
        // دریافت یک پیوست با شناسه
        Task<Attachment?> GetAttachmentByIdAsync(int id);
        
        // ایجاد پیوست جدید
        Task<Attachment> CreateAttachmentAsync(int letterId, string title, string description, List<IFormFile> files);
        
        // اضافه کردن فایل به پیوست
        Task AddFilesToAttachmentAsync(int attachmentId, List<IFormFile> files);
        
        // اضافه کردن بازنگری به پیوست
        Task<AttachmentRevision> AddRevisionAsync(int attachmentId, string status, string comment, string changedBy);
        
        // حذف پیوست
        Task DeleteAttachmentAsync(int id);
        
        // دانلود فایل پیوست
        Task<(Stream fileStream, string fileName, string contentType)> DownloadFileAsync(int fileId);
    }
} 