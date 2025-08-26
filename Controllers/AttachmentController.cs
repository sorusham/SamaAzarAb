using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using System.IO;

namespace MessageForAzarab.Controllers
{
    public class AttachmentController : Controller
    {
        private readonly IAttachmentService _attachmentService;
        private readonly ILetterService _letterService;
        private readonly IAttachmentRevisionService _revisionService;

        public AttachmentController(
            IAttachmentService attachmentService, 
            ILetterService letterService,
            IAttachmentRevisionService revisionService)
        {
            _attachmentService = attachmentService;
            _letterService = letterService;
            _revisionService = revisionService;
        }

        // نمایش فرم ایجاد پیوست جدید
        public async Task<IActionResult> Create(int letterId)
        {
            var letter = await _letterService.GetLetterByIdAsync(letterId);
            if (letter == null)
                return NotFound();

            var attachment = new Attachment { LetterId = letterId };
            return View(attachment);
        }

        // ذخیره پیوست جدید
        [HttpPost]
        public async Task<IActionResult> Create(int letterId, string title, string description, List<IFormFile> files)
        {
            var attachment = await _attachmentService.CreateAttachmentAsync(letterId, title, description, files);
            return RedirectToAction("Details", "Letters", new { id = letterId });
        }

        // نمایش جزئیات پیوست
        public async Task<IActionResult> Details(int id)
        {
            var attachment = await _attachmentService.GetAttachmentByIdAsync(id);

            if (attachment == null)
                return NotFound();

            return View(attachment);
        }

        // دانلود فایل
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var (fileStream, fileName, contentType) = await _attachmentService.DownloadFileAsync(id);
                return File(fileStream, contentType, fileName);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        // اضافه کردن بازنگری جدید
        [HttpPost]
        public async Task<IActionResult> AddRevision(int attachmentId, string status, string comment)
        {
            try
            {
                var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId);
                if (attachment == null)
                    return NotFound();

                var changedBy = User.Identity?.Name ?? "Unknown";
                await _revisionService.CreateRevisionAsync(attachmentId, status, comment, changedBy);

                return RedirectToAction("Details", "Letters", new { id = attachment.LetterId });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        // اضافه کردن فایل جدید به پیوست موجود
        [HttpPost]
        public async Task<IActionResult> AddFiles(int attachmentId, List<IFormFile> files)
        {
            try
            {
                var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId);
                if (attachment == null)
                    return NotFound();

                await _attachmentService.AddFilesToAttachmentAsync(attachmentId, files);

                return RedirectToAction("Details", nameof(AttachmentController).Replace("Controller", ""), new { id = attachmentId });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        // حذف پیوست
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
                if (attachment == null)
                    return NotFound();

                var letterId = attachment.LetterId;
                await _attachmentService.DeleteAttachmentAsync(id);

                return RedirectToAction("Details", "Letters", new { id = letterId });
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
} 