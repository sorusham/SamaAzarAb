using MessageForAzarab.Data;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MessageForAzarab.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public DocumentService(ApplicationDbContext context, IWebHostEnvironment environment, IFileService fileService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _environment = environment;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }
        
        #region مدیریت اسناد پایه
        
        public async Task<List<BaseDocument>> GetAllDocumentsAsync()
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }
        
        public async Task<List<BaseDocument>> GetActiveDocumentsAsync()
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .Where(d => d.IsActive && d.Status == "E" && !d.Hold)
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }
        
        public async Task<List<BaseDocument>> GetDocumentsByProjectAsync(int projectId)
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .Where(d => d.ProjectId == projectId)
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }
        
        public async Task<List<BaseDocument>> GetDocumentsByDepartmentAsync(int departmentId)
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId)
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }
        
        public async Task<BaseDocument?> GetDocumentByIdAsync(int id)
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .Include(d => d.DocumentVersions)
                    .ThenInclude(v => v.Attachments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        
        public async Task<BaseDocument?> GetDocumentByCodeAsync(string code)
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DocCode == code);
        }
        
        public async Task<BaseDocument> CreateDocumentAsync(BaseDocument document)
        {
            try
            {
                // بررسی تکراری بودن کد مدرک
                var existingDoc = await _context.BaseDocuments
                    .FirstOrDefaultAsync(d => d.DocCode == document.DocCode);
                
                if (existingDoc != null)
                {
                    throw new InvalidOperationException($"سند با کد {document.DocCode} قبلاً ثبت شده است.");
                }

                // تنظیم مقادیر پیش‌فرض
                document.CreationDate = DateTime.Now;
                document.LastModificationDate = DateTime.Now;
                document.Status = "E"; // فعال
                document.IsActive = true;
                document.CurrentRevision = 0;

                _context.BaseDocuments.Add(document);
                await _context.SaveChangesAsync();
                
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"خطا در ایجاد سند: {ex.Message}", ex);
            }
        }
        
        public async Task UpdateDocumentAsync(BaseDocument document)
        {
            try
            {
                var existingDocument = await _context.BaseDocuments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == document.Id);
                
                if (existingDocument == null)
                {
                    throw new KeyNotFoundException($"سند با شناسه {document.Id} یافت نشد.");
                }
                
                _context.Entry(document).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                // ثبت خطا و پرتاب مجدد
                Console.WriteLine($"خطا در بروزرسانی سند: {ex.Message}");
                throw;
            }
        }
        
        public async Task DeleteDocumentAsync(int id)
        {
            var document = await _context.BaseDocuments
                                     .Include(d => d.DocumentVersions)
                                     .ThenInclude(v => v.Attachments)
                                     .FirstOrDefaultAsync(d => d.Id == id);
            if (document != null)
            {
                // حذف فایل‌های فیزیکی تمام پیوست‌ها قبل از حذف سند
                foreach (var version in document.DocumentVersions)
                {
                    foreach (var attachment in version.Attachments)
                    {
                        try
                        {
                            await _fileService.DeleteFileAsync(attachment.FilePath); 
                        }
                        catch (Exception ex)
                        {
                           Console.WriteLine($"خطا در حذف فایل فیزیکی پیوست {attachment.Id}: {ex.Message}");
                           // ادامه می‌دهیم تا خود سند حذف شود
                        }
                    }
                }
                
                _context.BaseDocuments.Remove(document); // cascade delete باید فعال باشد یا دستی حذف کنیم
                await _context.SaveChangesAsync();
            }
        }
        
        #endregion
        
        #region مدیریت نسخه‌های سند
        
        public async Task<List<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId)
        {
            return await _context.DocumentVersions
                .Where(v => v.BaseDocumentId == documentId)
                .Include(v => v.Attachments)
                .OrderByDescending(v => v.RevisionNumber)
                .ToListAsync();
        }
        
        public async Task<DocumentVersion?> GetVersionByIdAsync(int versionId)
        {
            return await _context.DocumentVersions
                .Include(v => v.BaseDocument)
                .Include(v => v.Creator)
                .Include(v => v.AssignedTo)
                .Include(v => v.Attachments)
                .FirstOrDefaultAsync(v => v.Id == versionId);
        }
        
        public async Task<DocumentVersion> CreateDocumentVersionAsync(DocumentVersion version)
        {
            try
            {
                // بررسی وجود سند پایه
                var baseDocument = await _context.BaseDocuments
                    .FirstOrDefaultAsync(d => d.Id == version.BaseDocumentId);
                
                if (baseDocument == null)
                {
                    throw new KeyNotFoundException($"سند پایه با شناسه {version.BaseDocumentId} یافت نشد.");
                }

                // بررسی وضعیت سند پایه
                if (baseDocument.Status != "E")
                {
                    throw new InvalidOperationException("امکان ایجاد نسخه جدید برای سند غیرفعال وجود ندارد.");
                }

                // محاسبه شماره نسخه بعدی
                var maxRevision = await _context.DocumentVersions
                    .Where(v => v.BaseDocumentId == version.BaseDocumentId)
                    .MaxAsync(v => (int?)v.RevisionNumber) ?? -1;
                
                version.RevisionNumber = maxRevision + 1;

                // تنظیم مقادیر پیش‌فرض
                version.CreationDate = DateTime.Now;
                version.Status = "O"; // باز
                version.IsSent = false;
                version.IsHidden = false;
                version.Progress = 0;

                _context.DocumentVersions.Add(version);
                await _context.SaveChangesAsync();

                // به‌روزرسانی نسخه فعلی در سند پایه
                baseDocument.CurrentRevision = version.RevisionNumber;
                baseDocument.LastModificationDate = DateTime.Now;
                await _context.SaveChangesAsync();
                
                return version;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"خطا در ایجاد نسخه سند: {ex.Message}", ex);
            }
        }
        
        public async Task UpdateDocumentVersionAsync(DocumentVersion version)
        {
            try
            {
                var existingVersion = await _context.DocumentVersions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == version.Id);
                
                if (existingVersion == null)
                {
                    throw new KeyNotFoundException($"نسخه سند با شناسه {version.Id} یافت نشد.");
                }
                
                _context.Entry(version).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                // ثبت خطا و پرتاب مجدد
                Console.WriteLine($"خطا در بروزرسانی نسخه سند: {ex.Message}");
                throw;
            }
        }
        
        public async Task DeleteDocumentVersionAsync(int versionId)
        {
            var version = await _context.DocumentVersions
                                    .Include(v => v.Attachments)
                                    .FirstOrDefaultAsync(v => v.Id == versionId);
            if (version != null)
            {
                // حذف فایل‌های فیزیکی پیوست‌ها قبل از حذف نسخه
                foreach (var attachment in version.Attachments)
                {
                    try
                    {
                       await _fileService.DeleteFileAsync(attachment.FilePath);
                    }
                     catch (Exception ex)
                    {
                       Console.WriteLine($"خطا در حذف فایل فیزیکی پیوست {attachment.Id} در حذف نسخه: {ex.Message}");
                    }
                }
                _context.DocumentVersions.Remove(version);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task SendVersionAsync(int versionId)
        {
            var version = await _context.DocumentVersions.FindAsync(versionId);
            if (version != null)
            {
                if (version.IsSent)
                {
                    throw new InvalidOperationException("این نسخه قبلا ارسال شده است.");
                }
                version.IsSent = true;
                _context.Entry(version).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"نسخه با شناسه {versionId} یافت نشد.");
            }
        }
        
        #endregion
        
        #region مدیریت تراکنش‌های سند
        
        public async Task<List<DocumentTransaction>> GetTransactionsByVersionIdAsync(int versionId)
        {
            return await _context.DocumentTransactions
                .Where(t => t.DocumentVersionId == versionId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
        
        public async Task<DocumentTransaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _context.DocumentTransactions
                .Include(t => t.DocumentVersion)
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }
        
        public async Task<DocumentTransaction> CreateDocumentTransactionAsync(DocumentTransaction transaction)
        {
            _context.DocumentTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
        
        public async Task UpdateDocumentTransactionAsync(DocumentTransaction transaction)
        {
            try
            {
                var existingTransaction = await _context.DocumentTransactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == transaction.Id);
                
                if (existingTransaction == null)
                {
                    throw new KeyNotFoundException($"تراکنش سند با شناسه {transaction.Id} یافت نشد.");
                }
                
                _context.Entry(transaction).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                // ثبت خطا و پرتاب مجدد
                Console.WriteLine($"خطا در بروزرسانی تراکنش سند: {ex.Message}");
                throw;
            }
        }
        
        public async Task DeleteDocumentTransactionAsync(int transactionId)
        {
            var transaction = await _context.DocumentTransactions.FindAsync(transactionId);
            if (transaction != null)
            {
                _context.DocumentTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
        
        #endregion
        
        #region مدیریت فایل‌های پیوست سند
        
        public async Task<List<DocumentAttachment>> GetAttachmentsByVersionIdAsync(int versionId)
        {
            return await _context.DocumentAttachments
                .Where(a => a.DocumentVersionId == versionId)
                .OrderByDescending(a => a.UploadDate)
                .ToListAsync();
        }
        
        public async Task<DocumentAttachment?> GetAttachmentByIdAsync(int attachmentId)
        {
            return await _context.DocumentAttachments
                .Include(a => a.DocumentVersion)
                .Include(a => a.Uploader)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        }
        
        public async Task<DocumentAttachment> CreateDocumentAttachmentAsync(int baseDocumentId, int documentVersionId, IFormFile file)
        {
            try
            {
                // اعتبارسنجی فایل
                if (file == null || file.Length == 0)
                    throw new ArgumentException("فایل پیوست نامعتبر است.");

                // بررسی محدودیت حجم فایل (مثلاً 50 مگابایت)
                const long maxFileSize = 50 * 1024 * 1024; // 50 MB
                if (file.Length > maxFileSize)
                    throw new ArgumentException($"حجم فایل نمی‌تواند بیش از {maxFileSize / (1024 * 1024)} مگابایت باشد.");

                // بررسی نوع فایل مجاز
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".dwg", ".dxf", ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    throw new ArgumentException($"نوع فایل {fileExtension} مجاز نیست. فایل‌های مجاز: {string.Join(", ", allowedExtensions)}");

                // بررسی وجود نسخه سند
                var version = await GetVersionByIdAsync(documentVersionId);
                if (version == null)
                    throw new KeyNotFoundException($"نسخه سند با شناسه {documentVersionId} یافت نشد.");

                // بررسی وضعیت نسخه
                if (version.IsSent)
                    throw new InvalidOperationException("امکان اضافه کردن پیوست به نسخه ارسال شده وجود ندارد.");

                // بررسی وجود سند پایه
                var baseDocument = await _context.BaseDocuments
                    .FirstOrDefaultAsync(d => d.Id == baseDocumentId);
                if (baseDocument == null)
                    throw new KeyNotFoundException($"سند پایه با شناسه {baseDocumentId} یافت نشد.");

                // ذخیره فایل فیزیکی
                var relativePath = $"Documents/{baseDocumentId}/{documentVersionId}";
                var saveResult = await _fileService.SaveFileAsync(file, relativePath);

                // گرفتن کاربر فعلی
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // ایجاد رکورد دیتابیس
                var attachment = new DocumentAttachment
                {
                    DocumentVersionId = documentVersionId,
                    FileName = saveResult.OriginalFileName,
                    FilePath = saveResult.FullPath, 
                    FileSize = saveResult.FileSize,
                    ContentType = saveResult.ContentType,
                    UploadDate = DateTime.Now,
                    UploaderId = userId
                };
                
                _context.DocumentAttachments.Add(attachment);
                await _context.SaveChangesAsync();
                
                return attachment;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"خطا در ایجاد پیوست سند: {ex.Message}", ex);
            }
        }
        
        public async Task UpdateDocumentAttachmentAsync(DocumentAttachment attachment)
        {
            try
            {
                var existingAttachment = await _context.DocumentAttachments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == attachment.Id);
                
                if (existingAttachment == null)
                {
                    throw new KeyNotFoundException($"پیوست سند با شناسه {attachment.Id} یافت نشد.");
                }
                
                _context.Entry(attachment).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                // ثبت خطا و پرتاب مجدد
                Console.WriteLine($"خطا در بروزرسانی پیوست سند: {ex.Message}");
                throw;
            }
        }
        
        public async Task DeleteDocumentAttachmentAsync(int attachmentId)
        {
            var attachment = await _context.DocumentAttachments
                                     .Include(a => a.DocumentVersion)
                                     .FirstOrDefaultAsync(a => a.Id == attachmentId);
                                     
            if (attachment != null)
            {
                 if (attachment.DocumentVersion != null && attachment.DocumentVersion.IsSent)
                 {
                     throw new InvalidOperationException("امکان حذف پیوست از نسخه ارسال شده وجود ندارد.");
                 }

                try
                {
                    // حذف فایل فیزیکی
                    await _fileService.DeleteFileAsync(attachment.FilePath);
                }
                catch (FileNotFoundException) 
                {
                    // اگر فایل وجود نداشت، مشکلی نیست، فقط لاگ می‌گیریم
                    Console.WriteLine($"Warning: Physical file not found for attachment {attachmentId} during delete: {attachment.FilePath}");
                }
                catch (Exception ex)
                {
                    // خطای دیگر در حذف فایل، آن را لاگ می‌کنیم اما ادامه می‌دهیم
                    Console.WriteLine($"Error deleting physical file for attachment {attachmentId}: {ex.Message}");
                }

                _context.DocumentAttachments.Remove(attachment);
                await _context.SaveChangesAsync();
            }
            else
            {
                 throw new KeyNotFoundException($"پیوست با شناسه {attachmentId} یافت نشد.");
            }
        }
        
        #endregion
        
        #region مدیریت وضعیت سند
        
        public async Task<bool> UpdateDocumentStatusAsync(int baseDocumentId, string status)
        {
            var document = await _context.BaseDocuments.FindAsync(baseDocumentId);
            if (document != null)
            {
                document.Status = status;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        
        public async Task<bool> UpdateDocumentReviewStageAsync(int baseDocumentId, DocumentReviewStage stage)
        {
            var document = await _context.BaseDocuments.FindAsync(baseDocumentId);
            if (document != null)
            {
                document.ReviewStage = stage;
                
                // ثبت تاریخ بررسی یا تأیید
                if (stage == DocumentReviewStage.Checker)
                {
                    document.DateChecker = DateTime.Now;
                }
                else if (stage == DocumentReviewStage.Approver)
                {
                    document.DateApprover = DateTime.Now;
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        
        #endregion
        
        #region متدهای گزارش‌گیری
        
        public async Task<List<BaseDocument>> GetDocumentsWithStatusAsync(string status)
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .Where(d => d.Status == status)
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }
        
        public async Task<List<BaseDocument>> GetDocumentsByReviewStageAsync(DocumentReviewStage stage)
        {
            return await _context.BaseDocuments
                .Include(d => d.Department)
                .Where(d => d.ReviewStage == stage)
                .OrderByDescending(d => d.Id)
                .ToListAsync();
        }
        
        #endregion
    }
} 