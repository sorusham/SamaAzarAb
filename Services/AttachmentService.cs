using MessageForAzarab.Data;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MessageForAzarab.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;

        public AttachmentService(
            ApplicationDbContext context, 
            IFileService fileService,
            INotificationService notificationService)
        {
            _context = context;
            _fileService = fileService;
            _notificationService = notificationService;
        }

        public async Task<List<Attachment>> GetAttachmentsByLetterIdAsync(int letterId)
        {
            return await _context.Attachments
                .Include(a => a.Files)
                .Include(a => a.Revisions)
                .Where(a => a.LetterId == letterId)
                .ToListAsync();
        }

        public async Task<Attachment?> GetAttachmentByIdAsync(int id)
        {
            return await _context.Attachments
                .Include(a => a.Files)
                .Include(a => a.Revisions)
                .Include(a => a.Letter)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Attachment> CreateAttachmentAsync(int letterId, string title, string description, List<IFormFile> files)
        {
            var letter = await _context.Letters.FindAsync(letterId);
            if (letter == null)
                throw new ArgumentException("نامه‌ای با این شناسه یافت نشد");

            var attachment = new Attachment
            {
                LetterId = letterId,
                Title = title,
                Description = description,
                DateSent = DateTime.Now,
                From = letter.From,  // اضافه کردن فرستنده
                To = letter.To,       // اضافه کردن گیرنده
                SenderId = letter.SenderId, // اضافه کردن شناسه فرستنده
                ReceiverId = letter.ReceiverId // اضافه کردن شناسه گیرنده
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            // ذخیره فایل‌ها
            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    var saveResult = await _fileService.SaveFileAsync(file, letter.Code);
                    var attachmentFile = new AttachmentFile
                    {
                        AttachmentId = attachment.Id,
                        StoredFileName = saveResult.StoredFileName,
                        OriginalFileName = saveResult.OriginalFileName,
                        FilePath = saveResult.FullPath,
                        FileSize = saveResult.FileSize,
                        FileType = saveResult.ContentType
                    };
                    _context.AttachmentFiles.Add(attachmentFile);
                }
                await _context.SaveChangesAsync();
            }

            // ارسال اعلان به گیرنده
            if (!string.IsNullOrEmpty(attachment.ReceiverId) && attachment.ReceiverId != attachment.SenderId)
            {
                await _notificationService.CreateNotificationAsync(
                    attachment.ReceiverId,
                    "پیوست جدید",
                    $"یک پیوست جدید به نام «{attachment.Title}» برای نامه با کد «{letter.Code}» دریافت کرده‌اید.",
                    NotificationType.NewAttachment,
                    $"/Letters/Details/{letterId}"
                );
            }

            return attachment;
        }

        public async Task AddFilesToAttachmentAsync(int attachmentId, List<IFormFile> files)
        {
            var attachment = await _context.Attachments
                .Include(a => a.Letter)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            if (attachment == null)
                throw new ArgumentException("پیوستی با این شناسه یافت نشد");

            if (files != null && files.Any())
            {
                // Make sure the Letter Code exists 
                if (string.IsNullOrEmpty(attachment.Letter.Code))
                    throw new ArgumentException("کد نامه موجود نیست");

                foreach (var file in files)
                {
                    var saveResult = await _fileService.SaveFileAsync(file, attachment.Letter.Code);
                    var attachmentFile = new AttachmentFile
                    {
                        AttachmentId = attachment.Id,
                        StoredFileName = saveResult.StoredFileName,
                        OriginalFileName = saveResult.OriginalFileName,
                        FilePath = saveResult.FullPath,
                        FileSize = saveResult.FileSize,
                        FileType = saveResult.ContentType
                    };
                    _context.AttachmentFiles.Add(attachmentFile);
                }
                await _context.SaveChangesAsync();

                // ارسال اعلان به گیرنده
                if (!string.IsNullOrEmpty(attachment.ReceiverId) && attachment.ReceiverId != attachment.SenderId)
                {
                    await _notificationService.CreateNotificationAsync(
                        attachment.ReceiverId,
                        "فایل‌های جدید اضافه شد",
                        $"{files.Count} فایل جدید به پیوست «{attachment.Title}» اضافه شده است.",
                        NotificationType.NewAttachment,
                        $"/Attachment/Details/{attachmentId}"
                    );
                }
            }
        }

        public async Task<AttachmentRevision> AddRevisionAsync(int attachmentId, string status, string comment, string changedBy)
        {
            var attachment = await _context.Attachments
                .Include(a => a.Revisions)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            if (attachment == null)
                throw new ArgumentException("پیوستی با این شناسه یافت نشد");

            var revision = new AttachmentRevision
            {
                AttachmentId = attachmentId,
                RevisionNumber = attachment.Revisions.Count,
                Status = status,
                Comment = comment,
                StatusDate = DateTime.Now,
                ChangedBy = changedBy,
                From = attachment.From ?? string.Empty,  // اضافه کردن فرستنده با بررسی null
                To = attachment.To ?? string.Empty,      // اضافه کردن گیرنده با بررسی null
                SenderId = attachment.SenderId, // اضافه کردن شناسه فرستنده
                ReceiverId = attachment.ReceiverId // اضافه کردن شناسه گیرنده
            };

            _context.AttachmentRevisions.Add(revision);
            await _context.SaveChangesAsync();

            return revision;
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            var attachment = await _context.Attachments
                .Include(a => a.Files)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attachment == null)
                throw new ArgumentException("پیوستی با این شناسه یافت نشد");

            // حذف فایل‌های پیوست
            foreach (var file in attachment.Files)
            {
                await _fileService.DeleteFileAsync(file.FilePath);
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task<(Stream fileStream, string fileName, string contentType)> DownloadFileAsync(int fileId)
        {
            var file = await _context.AttachmentFiles
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (file == null)
                throw new ArgumentException("فایلی با این شناسه یافت نشد");

            var stream = await _fileService.GetFileStreamAsync(file.FilePath);
            return (stream, file.OriginalFileName, file.FileType);
        }
    }
} 