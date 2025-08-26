using MessageForAzarab.Data;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MessageForAzarab.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetActiveUsersAsync()
        {
            return await _userManager.Users.Where(u => u.IsActive).ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetUsersByTypeAsync(UserType userType)
        {
            return await _userManager.Users.Where(u => u.UserType == userType).ToListAsync();
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task SetUserActiveStatusAsync(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = isActive;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            return await _userManager.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<List<Letter>> GetUserSentLettersAsync(string userId)
        {
            return await _context.Letters
                .Include(l => l.Attachments)
                .Where(l => l.SenderId == userId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<List<Letter>> GetUserReceivedLettersAsync(string userId)
        {
            return await _context.Letters
                .Include(l => l.Attachments)
                .Where(l => l.ReceiverId == userId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<List<Attachment>> GetUserSentAttachmentsAsync(string userId)
        {
            return await _context.Attachments
                .Include(a => a.Letter)
                .Include(a => a.Files)
                .Include(a => a.Revisions)
                .Where(a => a.SenderId == userId)
                .OrderByDescending(a => a.DateSent)
                .ToListAsync();
        }

        public async Task<List<Attachment>> GetUserReceivedAttachmentsAsync(string userId)
        {
            return await _context.Attachments
                .Include(a => a.Letter)
                .Include(a => a.Files)
                .Include(a => a.Revisions)
                .Where(a => a.ReceiverId == userId)
                .OrderByDescending(a => a.DateSent)
                .ToListAsync();
        }

        public async Task<List<AttachmentRevision>> GetUserSentRevisionsAsync(string userId)
        {
            return await _context.AttachmentRevisions
                .Include(r => r.Attachment)
                .ThenInclude(a => a.Letter)
                .Where(r => r.SenderId == userId)
                .OrderByDescending(r => r.StatusDate)
                .ToListAsync();
        }

        public async Task<List<AttachmentRevision>> GetUserReceivedRevisionsAsync(string userId)
        {
            return await _context.AttachmentRevisions
                .Include(r => r.Attachment)
                .ThenInclude(a => a.Letter)
                .Where(r => r.ReceiverId == userId)
                .OrderByDescending(r => r.StatusDate)
                .ToListAsync();
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }
    }
} 