using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MessageForAzarab.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadBasePath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _uploadBasePath = configuration["FileStorage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(_uploadBasePath))
            {
                Directory.CreateDirectory(_uploadBasePath);
            }
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<FileSaveResult> SaveFileAsync(IFormFile file, string relativePath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("فایل نامعتبر است یا خالی است.");

            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("مسیر نسبی برای ذخیره فایل نامعتبر است.");
                
            relativePath = relativePath.Replace("..", "");
            relativePath = Path.GetFullPath(Path.Combine("uploads", relativePath)).Substring(Path.GetFullPath(Path.Combine("uploads", "")).Length);

            var fileExtension = Path.GetExtension(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}{fileExtension}";
            var finalRelativePath = Path.Combine(Path.GetDirectoryName(relativePath) ?? string.Empty, storedFileName).Replace("\\", "/");

            var fullDirectoryPath = Path.Combine(_uploadBasePath, Path.GetDirectoryName(relativePath) ?? string.Empty);
            var fullPath = Path.Combine(fullDirectoryPath, storedFileName);

            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new FileSaveResult(
                StoredFileName: storedFileName,
                OriginalFileName: file.FileName,
                RelativePath: finalRelativePath,
                FullPath: fullPath,
                FileSize: file.Length,
                ContentType: file.ContentType
            );
        }

        public async Task<Stream> GetFileStreamAsync(string fullPath)
        {
            var sanitizedFullPath = Path.GetFullPath(fullPath);
            if (!sanitizedFullPath.StartsWith(Path.GetFullPath(_uploadBasePath)))
            {
                 throw new UnauthorizedAccessException("دسترسی به مسیر فایل مجاز نیست.");
            }
            
            if (string.IsNullOrWhiteSpace(sanitizedFullPath) || !File.Exists(sanitizedFullPath))
                throw new FileNotFoundException("فایل یافت نشد یا مسیر نامعتبر است.", sanitizedFullPath);

            var stream = new FileStream(sanitizedFullPath, FileMode.Open, FileAccess.Read);
            return await Task.FromResult(stream);
        }

        public async Task DeleteFileAsync(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                 throw new ArgumentException("مسیر فایل برای حذف نامعتبر است.");

            var sanitizedFullPath = Path.GetFullPath(fullPath);
             if (!sanitizedFullPath.StartsWith(Path.GetFullPath(_uploadBasePath)))
            {
                 Console.WriteLine($"Warning: Attempt to delete file outside base path: {fullPath}");
                 throw new UnauthorizedAccessException("اجازه حذف فایل در این مسیر وجود ندارد.");
            }

            if (File.Exists(sanitizedFullPath))
            {
                try
                {
                    File.Delete(sanitizedFullPath);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error deleting file {sanitizedFullPath}: {ex.Message}");
                    throw;
                }
            }
            else
            {
                 Console.WriteLine($"Warning: File not found for deletion: {sanitizedFullPath}");
            }
            await Task.CompletedTask;
        }

        public string GetFileUrl(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                 throw new ArgumentException("مسیر نسبی فایل نامعتبر است.");
                 
            relativePath = relativePath.Replace("..", "").Replace("\\", "/");
            if (relativePath.StartsWith("/"))
            {
                relativePath = relativePath.Substring(1);
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new InvalidOperationException("HTTP context در دسترس نیست.");

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            var uploadUrlBase = "/uploads";
            
            return $"{baseUrl}{uploadUrlBase}/{relativePath}";
        }
    }
} 