using MessageForAzarab.Models;
using System.Security.Claims;

namespace MessageForAzarab.Services.Interface
{
    public interface IUserService
    {
        // دریافت کاربر با شناسه
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        
        // دریافت کاربر با نام کاربری
        Task<ApplicationUser?> GetUserByUserNameAsync(string userName);
        
        // دریافت کاربر با ایمیل
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        
        // دریافت همه کاربران
        Task<List<ApplicationUser>> GetAllUsersAsync();
        
        // دریافت کاربران فعال
        Task<List<ApplicationUser>> GetActiveUsersAsync();
        
        // دریافت کاربران با نوع مشخص
        Task<List<ApplicationUser>> GetUsersByTypeAsync(UserType userType);
        
        // بروزرسانی کاربر
        Task UpdateUserAsync(ApplicationUser user);
        
        // فعال/غیرفعال کردن کاربر
        Task SetUserActiveStatusAsync(string userId, bool isActive);
        
        // بررسی وجود کاربر
        Task<bool> UserExistsAsync(string userId);
        
        // دریافت نامه‌های ارسال شده کاربر
        Task<List<Letter>> GetUserSentLettersAsync(string userId);
        
        // دریافت نامه‌های دریافت شده کاربر
        Task<List<Letter>> GetUserReceivedLettersAsync(string userId);
        
        // دریافت پیوست‌های ارسال شده کاربر
        Task<List<Attachment>> GetUserSentAttachmentsAsync(string userId);
        
        // دریافت پیوست‌های دریافت شده کاربر
        Task<List<Attachment>> GetUserReceivedAttachmentsAsync(string userId);
        
        // دریافت بازنگری‌های ارسال شده کاربر
        Task<List<AttachmentRevision>> GetUserSentRevisionsAsync(string userId);
        
        // دریافت بازنگری‌های دریافت شده کاربر
        Task<List<AttachmentRevision>> GetUserReceivedRevisionsAsync(string userId);
        
        // دریافت کاربر فعلی
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user);
    }
} 