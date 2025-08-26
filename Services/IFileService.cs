using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
// using MessageForAzarab.Models; // حذف وابستگی به مدل خاص

namespace MessageForAzarab.Services
{
    // نتیجه ذخیره فایل
    public record FileSaveResult(
        string StoredFileName, 
        string OriginalFileName, 
        string RelativePath, // مسیر نسبی از wwwroot/uploads برای URL و ذخیره در دیتابیس
        string FullPath,    // مسیر کامل فیزیکی برای خواندن/حذف
        long FileSize, 
        string ContentType
    );

    public interface IFileService
    {
        // ذخیره فایل با مسیر نسبی مشخص
        Task<FileSaveResult> SaveFileAsync(IFormFile file, string relativePath);
        
        // دریافت استریم فایل با مسیر کامل
        Task<Stream> GetFileStreamAsync(string fullPath);
        
        // حذف فایل با مسیر کامل
        Task DeleteFileAsync(string fullPath);
        
        // دریافت URL عمومی فایل با مسیر نسبی
        string GetFileUrl(string relativePath);
    }
}
