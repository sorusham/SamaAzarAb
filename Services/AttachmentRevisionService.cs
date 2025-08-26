using MessageForAzarab.Data;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MessageForAzarab.Services
{
    public class AttachmentRevisionService : IAttachmentRevisionService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public AttachmentRevisionService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<AttachmentRevision>> GetRevisionsByAttachmentIdAsync(int attachmentId)
        {
            return await _context.AttachmentRevisions
                .Where(r => r.AttachmentId == attachmentId)
                .OrderBy(r => r.RevisionNumber)
                .ToListAsync();
        }

        public async Task<AttachmentRevision?> GetRevisionByIdAsync(int id)
        {
            return await _context.AttachmentRevisions
                .Include(r => r.Attachment)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<AttachmentRevision> CreateRevisionAsync(int attachmentId, string status, string comment, string changedBy)
        {
            var attachment = await _context.Attachments
                .Include(a => a.Revisions)
                .Include(a => a.Letter)
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

            // ارسال اعلان به فرستنده و گیرنده
            if (!string.IsNullOrEmpty(attachment.SenderId))
            {
                await _notificationService.CreateNotificationAsync(
                    attachment.SenderId,
                    "بازنگری جدید",
                    $"یک بازنگری جدید برای پیوست «{attachment.Title}» نامه «{attachment.Letter.Title}» با وضعیت «{status}» ایجاد شد.",
                    NotificationType.NewRevision,
                    $"/Attachment/Details/{attachmentId}"
                );
            }

            if (!string.IsNullOrEmpty(attachment.ReceiverId) && attachment.ReceiverId != attachment.SenderId)
            {
                await _notificationService.CreateNotificationAsync(
                    attachment.ReceiverId,
                    "بازنگری جدید",
                    $"یک بازنگری جدید برای پیوست «{attachment.Title}» نامه «{attachment.Letter.Title}» با وضعیت «{status}» ایجاد شد.",
                    NotificationType.NewRevision,
                    $"/Attachment/Details/{attachmentId}"
                );
            }

            return revision;
        }

        public async Task<AttachmentRevision> UpdateRevisionStatusAsync(int revisionId, string newStatus, string comment, string changedBy)
        {
            var revision = await _context.AttachmentRevisions
                .FirstOrDefaultAsync(r => r.Id == revisionId);

            if (revision == null)
                throw new ArgumentException("بازنگری با این شناسه یافت نشد");

            revision.Status = newStatus;
            revision.Comment = comment;
            revision.ChangedBy = changedBy;
            revision.StatusDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return revision;
        }

        public async Task DeleteRevisionAsync(int id)
        {
            var revision = await _context.AttachmentRevisions
                .FirstOrDefaultAsync(r => r.Id == id);

            if (revision == null)
                throw new ArgumentException("بازنگری با این شناسه یافت نشد");

            _context.AttachmentRevisions.Remove(revision);
            await _context.SaveChangesAsync();
        }
    }
} 