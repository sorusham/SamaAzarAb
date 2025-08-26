using System.Security.Cryptography.X509Certificates;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.EntityFrameworkCore;
using MessageForAzarab.Data;

namespace MessageForAzarab.Services
{
    public class LetterService : ILetterService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public LetterService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<Letter>> GetAllLettersAsync()
        {
            return await _context.Letters
                .Include(l => l.Attachments)
                .ThenInclude(a => a.Files)
                .Include(l => l.Attachments)
                .ThenInclude(a => a.Revisions)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<Letter?> GetLetterByIdAsync(int id)
        {
            return await _context.Letters
                .Include(l => l.Attachments)
                .ThenInclude(a => a.Files)
                .Include(l => l.Attachments)
                .ThenInclude(a => a.Revisions)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Letter> CreateLetterAsync(Letter letter)
        {
            letter.Code = GenerateLetterCode();
            _context.Letters.Add(letter);
            await _context.SaveChangesAsync();

            // ارسال اعلان به گیرنده
            if (!string.IsNullOrEmpty(letter.ReceiverId) && letter.ReceiverId != letter.SenderId)
            {
                await _notificationService.CreateNotificationAsync(
                    letter.ReceiverId,
                    "نامه جدید",
                    $"یک نامه جدید با عنوان «{letter.Title}» و کد «{letter.Code}» دریافت کرده‌اید.",
                    NotificationType.NewLetter,
                    $"/Letters/Details/{letter.Id}"
                );
            }

            return letter;
        }

        public async Task AddAttachmentAsync(int letterId, Attachment attachment)
        {
            var letter = await _context.Letters.FindAsync(letterId);
            if (letter != null)
            {
                attachment.LetterId = letterId;
                attachment.DateSent = DateTime.Now;
                _context.Attachments.Add(attachment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLetterAsync(Letter letter)
        {
            try
            {
                // Clear any previous tracked changes to avoid multiple entity tracking
                _context.ChangeTracker.Clear();
                
                // Get the current letter from database
                var existingLetter = await _context.Letters
                    .Include(l => l.Sender)
                    .Include(l => l.Receiver)
                    .FirstOrDefaultAsync(l => l.Id == letter.Id);
                    
                if (existingLetter == null)
                {
                    throw new InvalidOperationException($"Letter with ID {letter.Id} not found");
                }
                
                // Update the existing letter properties
                existingLetter.Title = letter.Title;
                existingLetter.Description = letter.Description;
                existingLetter.From = letter.From;
                existingLetter.To = letter.To;
                existingLetter.StartDate = letter.StartDate;
                existingLetter.ExpiryDate = letter.ExpiryDate;
                existingLetter.SenderId = letter.SenderId;
                existingLetter.ReceiverId = letter.ReceiverId;
                
                // Save changes to the database
                await _context.SaveChangesAsync();
                
                // Send notification to receiver if different from sender
                if (existingLetter.SenderId != existingLetter.ReceiverId && !string.IsNullOrEmpty(existingLetter.ReceiverId))
                {
                    await SendNotificationToReceiverAsync(existingLetter);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LetterExists(letter.Id))
                {
                    throw new InvalidOperationException($"Letter with ID {letter.Id} not found");
                }
                throw;
            }
        }

        public async Task DeleteLetterAsync(int id)
        {
            var letter = await _context.Letters
                .Include(l => l.Attachments)
                .ThenInclude(a => a.Files)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (letter != null)
            {
                _context.Letters.Remove(letter);
                await _context.SaveChangesAsync();
            }
        }

        // Generate a unique code for letters
        private string GenerateLetterCode()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("D2");
            var day = DateTime.Now.Day.ToString("D2");
            var random = new Random();
            var randomNumber = random.Next(1000, 9999);
            return $"{year}{month}{day}-{randomNumber}";
        }

        private bool LetterExists(int id)
        {
            return _context.Letters.Any(e => e.Id == id);
        }

        private async Task SendNotificationToReceiverAsync(Letter letter)
        {
            if (letter.Receiver != null && letter.ReceiverId != null)
            {
                await _notificationService.CreateNotificationAsync(
                    userId: letter.ReceiverId,
                    title: "بروزرسانی نامه",
                    message: $"نامه با عنوان \"{letter.Title}\" توسط {letter.From} بروزرسانی شد.",
                    type: NotificationType.DocumentUpdate,
                    link: $"/Letters/Details/{letter.Id}"
                );
            }
        }
    }
}
