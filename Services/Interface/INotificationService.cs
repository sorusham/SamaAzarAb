using MessageForAzarab.Models;

namespace MessageForAzarab.Services.Interface
{
    public interface INotificationService
    {
        // دریافت همه اعلان‌های یک کاربر
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        
        // دریافت اعلان‌های خوانده نشده کاربر
        Task<List<Notification>> GetUnreadNotificationsAsync(string userId);
        
        // تعداد اعلان‌های خوانده نشده کاربر
        Task<int> GetUnreadNotificationCountAsync(string userId);
        
        // ایجاد اعلان جدید
        Task CreateNotificationAsync(string userId, string title, string message, NotificationType type, string? link = null);
        
        // علامت‌گذاری اعلان به عنوان خوانده شده
        Task MarkAsReadAsync(int notificationId);
        
        // علامت‌گذاری همه اعلان‌های کاربر به عنوان خوانده شده
        Task MarkAllAsReadAsync(string userId);
        
        // حذف یک اعلان
        Task DeleteNotificationAsync(int notificationId);
    }
} 