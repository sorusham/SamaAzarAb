using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MessageForAzarab.Models;
using MessageForAzarab.Services.Interface;
using MessageForAzarab.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace MessageForAzarab.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentController(
            IDocumentService documentService,
            IUserService userService,
            IDepartmentService departmentService,
            IProjectService projectService,
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            IFileService fileService,
            IWebHostEnvironment webHostEnvironment)
        {
            _documentService = documentService;
            _userService = userService;
            _departmentService = departmentService;
            _projectService = projectService;
            _notificationService = notificationService;
            _userManager = userManager;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }

        // نمایش لیست اسناد
        public async Task<IActionResult> Index()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return View(documents);
        }

        // نمایش جزئیات سند
        public async Task<IActionResult> Details(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound();

            var users = await _userService.GetActiveUsersAsync();
            ViewBag.AssigneeId = new SelectList(users, "Id", "FullName");

            return View(document);
        }

        // ایجاد سند جدید
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var projects = await _projectService.GetAllProjectsAsync();
            var users = await _userService.GetActiveUsersAsync();
            
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name");
            ViewBag.ProjectId = new SelectList(projects, "Id", "ProjectCode");
            ViewBag.CheckerId = new SelectList(users, "Id", "FullName");
            ViewBag.ApproverId = new SelectList(users, "Id", "FullName");
            
            return View(new BaseDocument
            {
                Status = "E", // فعال
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BaseDocument document)
        {
            try
            {
                // بررسی تکراری بودن کد مدرک
                if (!string.IsNullOrEmpty(document.DocCode))
                {
                    var existingDoc = await _documentService.GetDocumentByCodeAsync(document.DocCode);
                    if (existingDoc != null)
                    {
                        ModelState.AddModelError("DocCode", "کد مدرک آذرآبی تکراری است.");
                    }
                }

                // دریافت کاربر فعلی
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) 
                {
                    TempData["ErrorMessage"] = "کاربر وارد شده یافت نشد.";
                    return Challenge();
                }

                // مقداردهی فیلدهای سیستم
                document.CreatorId = currentUser.Id;
                document.LastModifierId = currentUser.Id;
                document.CreationDate = DateTime.Now;
                document.LastModificationDate = DateTime.Now;

                // پاک‌کردن ارورهای غیرضروری
                ModelState.Remove(nameof(BaseDocument.Creator));
                ModelState.Remove(nameof(BaseDocument.LastModifier));
                ModelState.Remove(nameof(BaseDocument.Department));
                ModelState.Remove(nameof(BaseDocument.Project));
                ModelState.Remove(nameof(BaseDocument.CreatorId));
                ModelState.Remove(nameof(BaseDocument.LastModifierId));

                if (ModelState.IsValid)
                {
                    var createdDocument = await _documentService.CreateDocumentAsync(document);
                    
                    // ارسال اعلان‌ها
                    await SendNotificationsForNewDocument(createdDocument);
                    
                    TempData["SuccessMessage"] = "سند با موفقیت ایجاد شد.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطا در ایجاد سند: {ex.Message}";
            }

            // در صورت خطا، بارگذاری مجدد ViewBag ها
            await LoadCreateViewBags(document);
            return View(document);
        }

        private async Task SendNotificationsForNewDocument(BaseDocument document)
        {
            try
            {
                if (!string.IsNullOrEmpty(document.CheckerId))
                {
                    await _notificationService.CreateNotificationAsync(
                        document.CheckerId,
                        "سند جدید برای بررسی",
                        $"سند «{document.Title}» ({document.DocCode}) برای بررسی به شما ارجاع شد.",
                        NotificationType.NewDocument,
                        $"/Document/Details/{document.Id}"
                    );
                }
                
                if (!string.IsNullOrEmpty(document.ApproverId))
                {
                    await _notificationService.CreateNotificationAsync(
                        document.ApproverId,
                        "سند جدید برای تأیید",
                        $"سند «{document.Title}» ({document.DocCode}) برای تأیید به شما ارجاع شد.",
                        NotificationType.NewDocument,
                        $"/Document/Details/{document.Id}"
                    );
                }
            }
            catch (Exception ex)
            {
                // لاگ کردن خطا اما ادامه دادن فرآیند
                Console.WriteLine($"خطا در ارسال اعلان: {ex.Message}");
            }
        }

        private async Task LoadCreateViewBags(BaseDocument document)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var projects = await _projectService.GetAllProjectsAsync();
            var users = await _userService.GetActiveUsersAsync();
            
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name", document.DepartmentId);
            ViewBag.ProjectId = new SelectList(projects, "Id", "ProjectCode", document.ProjectId);
            ViewBag.CheckerId = new SelectList(users, "Id", "FullName", document.CheckerId);
            ViewBag.ApproverId = new SelectList(users, "Id", "FullName", document.ApproverId);
        }

        // ویرایش سند
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound();

            var departments = await _departmentService.GetAllDepartmentsAsync();
            var projects = await _projectService.GetAllProjectsAsync();
            var users = await _userService.GetActiveUsersAsync();
            
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name", document.DepartmentId);
            ViewBag.ProjectId = new SelectList(projects, "Id", "ProjectCode", document.ProjectId);
            ViewBag.CheckerId = new SelectList(users, "Id", "FullName", document.CheckerId);
            ViewBag.ApproverId = new SelectList(users, "Id", "FullName", document.ApproverId);
            
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BaseDocument document)
        {
            if (id != document.Id)
                return NotFound();

            try
            {
                // بررسی تکراری بودن کد مدرک
                if (!string.IsNullOrEmpty(document.DocCode))
                {
                    var existingDocWithCode = await _documentService.GetDocumentByCodeAsync(document.DocCode);
                    if (existingDocWithCode != null && existingDocWithCode.Id != id)
                    {
                        ModelState.AddModelError("DocCode", "کد مدرک آذرآبی تکراری است.");
                    }
                }

                // دریافت کاربر فعلی
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "کاربر وارد شده یافت نشد.";
                    return Challenge();
                }

                // دریافت سند قدیمی
                var oldDocument = await _documentService.GetDocumentByIdAsync(id);
                if (oldDocument == null) 
                {
                    TempData["ErrorMessage"] = "سند مورد نظر یافت نشد.";
                    return NotFound();
                }

                // به‌روزرسانی فیلدهای سیستم
                document.LastModifierId = currentUser.Id;
                document.LastModificationDate = DateTime.Now;
                document.CreatorId = oldDocument.CreatorId; // حفظ ایجادکننده اصلی
                document.CreationDate = oldDocument.CreationDate; // حفظ تاریخ ایجاد اصلی

                // پاک‌کردن ارورهای غیرضروری
                ModelState.Remove(nameof(BaseDocument.Creator));
                ModelState.Remove(nameof(BaseDocument.LastModifier));
                ModelState.Remove(nameof(BaseDocument.Department));
                ModelState.Remove(nameof(BaseDocument.Project));
                ModelState.Remove(nameof(BaseDocument.CreatorId));
                ModelState.Remove(nameof(BaseDocument.LastModifierId));

                if (ModelState.IsValid)
                {
                    await _documentService.UpdateDocumentAsync(document);
                    
                    // ارسال اعلان‌ها در صورت تغییر مسئولان
                    await SendNotificationsForUpdatedDocument(document, oldDocument);

                    TempData["SuccessMessage"] = "سند با موفقیت به‌روزرسانی شد.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (KeyNotFoundException) 
            { 
                TempData["ErrorMessage"] = "سند مورد نظر یافت نشد.";
                return NotFound(); 
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطا در به‌روزرسانی سند: {ex.Message}";
            }

            // در صورت خطا، بارگذاری مجدد ViewBag ها
            await LoadEditViewBags(document);
            return View(document);
        }

        private async Task SendNotificationsForUpdatedDocument(BaseDocument newDocument, BaseDocument oldDocument)
        {
            try
            {
                // اعلان برای تغییر صحه‌گذار
                if (!string.IsNullOrEmpty(newDocument.CheckerId) && oldDocument.CheckerId != newDocument.CheckerId)
                {
                    await _notificationService.CreateNotificationAsync(
                        newDocument.CheckerId,
                        "ارجاع سند برای بررسی",
                        $"سند «{newDocument.Title}» ({newDocument.DocCode}) برای بررسی به شما ارجاع شد.",
                        NotificationType.DocumentUpdate,
                        $"/Document/Details/{newDocument.Id}"
                    );
                }
                
                // اعلان برای تغییر تصدیق‌کننده
                if (!string.IsNullOrEmpty(newDocument.ApproverId) && oldDocument.ApproverId != newDocument.ApproverId)
                {
                    await _notificationService.CreateNotificationAsync(
                        newDocument.ApproverId,
                        "ارجاع سند برای تأیید",
                        $"سند «{newDocument.Title}» ({newDocument.DocCode}) برای تأیید به شما ارجاع شد.",
                        NotificationType.DocumentUpdate,
                        $"/Document/Details/{newDocument.Id}"
                    );
                }
            }
            catch (Exception ex)
            {
                // لاگ کردن خطا اما ادامه دادن فرآیند
                Console.WriteLine($"خطا در ارسال اعلان: {ex.Message}");
            }
        }

        private async Task LoadEditViewBags(BaseDocument document)
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var projects = await _projectService.GetAllProjectsAsync();
            var users = await _userService.GetActiveUsersAsync();
            
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name", document.DepartmentId);
            ViewBag.ProjectId = new SelectList(projects, "Id", "ProjectCode", document.ProjectId);
            ViewBag.CheckerId = new SelectList(users, "Id", "FullName", document.CheckerId);
            ViewBag.ApproverId = new SelectList(users, "Id", "FullName", document.ApproverId);
        }

        // حذف سند
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
                return NotFound();

            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _documentService.DeleteDocumentAsync(id);
                TempData["SuccessMessage"] = "سند و تمام نسخه‌ها و پیوست‌های آن با موفقیت حذف شد.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطا در حذف سند: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // نمایش مودال ایجاد نسخه جدید به جای صفحه جداگانه
        [HttpGet]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> CreateVersion(int documentId)
        {
            if (documentId <= 0)
                return NotFound("شناسه سند نامعتبر است.");
        
            var document = await _documentService.GetDocumentByIdAsync(documentId);
            if (document == null)
                return NotFound($"سند با شناسه {documentId} یافت نشد.");
        
            int nextRevisionNumber = (document.DocumentVersions?.Any() ?? false) ? document.DocumentVersions.Max(v => v.RevisionNumber) + 1 : 0;
            var currentUserId = _userManager.GetUserId(User);
            var newVersion = new DocumentVersion
            {
                BaseDocumentId = documentId,
                RevisionNumber = nextRevisionNumber,
                CreationDate = DateTime.Now,
                CreatorId = currentUserId ,
            };

            var users = await _userService.GetActiveUsersAsync();
            ViewBag.AssignedToId = new SelectList(users, "Id", "FullName");
            ViewBag.BaseDocumentTitle = document.Title;

            return PartialView("_CreateVersionPartial", newVersion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vendor")]
        public async Task<IActionResult> CreateVersion(DocumentVersion version, IFormFile? attachmentFile)
        {
            try
            {
                // بررسی وجود سند پایه
                var baseDoc = await _documentService.GetDocumentByIdAsync(version.BaseDocumentId);
                if (baseDoc == null)
                {
                    ModelState.AddModelError("", $"سند پایه با شناسه {version.BaseDocumentId} یافت نشد.");
                    await LoadCreateVersionViewBags(version, "نامشخص");
                    return PartialView("_CreateVersionPartial", version);
                }

                // بررسی وضعیت سند پایه
                if (baseDoc.Status != "E")
                {
                    ModelState.AddModelError("", "امکان ایجاد نسخه جدید برای سند غیرفعال وجود ندارد.");
                    await LoadCreateVersionViewBags(version, baseDoc.Title);
                    return PartialView("_CreateVersionPartial", version);
                }

                // دریافت کاربر فعلی
                var currentUserId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    ModelState.AddModelError("", "کاربر وارد شده یافت نشد.");
                    await LoadCreateVersionViewBags(version, baseDoc.Title);
                    return PartialView("_CreateVersionPartial", version);
                }

                // مقداردهی فیلدهای سیستم
                version.CreatorId = currentUserId;
                version.CreationDate = DateTime.Now;
                version.IsSent = false;
                version.Status = "O"; // باز

                // پاک‌کردن ارورهای غیرضروری
                ModelState.Remove(nameof(DocumentVersion.Creator));
                ModelState.Remove(nameof(DocumentVersion.AssignedTo));
                ModelState.Remove(nameof(DocumentVersion.BaseDocument));
                ModelState.Remove(nameof(DocumentVersion.CreatorId));

                if (ModelState.IsValid)
                {
                    var createdVersion = await _documentService.CreateDocumentVersionAsync(version);

                    // آپلود فایل پیوست در صورت وجود
                    if (attachmentFile != null && attachmentFile.Length > 0)
                    {
                        try
                        {
                            await _documentService.CreateDocumentAttachmentAsync(createdVersion.BaseDocumentId, createdVersion.Id, attachmentFile);
                            TempData["SuccessMessagePartial"] = "نسخه جدید و پیوست آن با موفقیت ایجاد شد.";
                        }
                        catch (Exception ex)
                        {
                            TempData["WarningMessagePartial"] = $"نسخه جدید ایجاد شد، اما در ذخیره پیوست خطا رخ داد: {ex.Message}";
                        }
                    }
                    else
                    {
                        TempData["SuccessMessagePartial"] = "نسخه جدید با موفقیت ایجاد شد.";
                    }
                    
                    return Json(new { success = true, redirectUrl = Url.Action("Details", "Document", new { id = version.BaseDocumentId }) });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"خطا در ایجاد نسخه جدید: {ex.Message}");
            }

            // در صورت خطا، بارگذاری مجدد ViewBag ها
            var docForTitle = await _documentService.GetDocumentByIdAsync(version.BaseDocumentId);
            await LoadCreateVersionViewBags(version, docForTitle?.Title ?? "نامشخص");
            return PartialView("_CreateVersionPartial", version);
        }

        private async Task LoadCreateVersionViewBags(DocumentVersion version, string baseDocumentTitle)
        {
            var users = await _userService.GetActiveUsersAsync();
            ViewBag.AssignedToId = new SelectList(users, "Id", "FullName", version.AssignedToId);
            ViewBag.BaseDocumentTitle = baseDocumentTitle;
        }
        
        // نمایش جزئیات نسخه
        [HttpGet]
        public async Task<IActionResult> VersionDetails(int id)
        {
            var version = await _documentService.GetVersionByIdAsync(id);
            if (version == null)
                return NotFound();

            return View(version);
        }

        // ویرایش نسخه
        [HttpGet]
        public async Task<IActionResult> EditVersion(int id)
        {
            var version = await _documentService.GetVersionByIdAsync(id);
            if (version == null)
                return NotFound();

            if (version.IsSent)
            {
                TempData["ErrorMessage"] = "امکان ویرایش نسخه ارسال شده وجود ندارد.";
                return RedirectToAction(nameof(VersionDetails), new { id = id });
            }

            var users = await _userService.GetActiveUsersAsync();
            ViewBag.AssignedToId = new SelectList(users, "Id", "FullName", version.AssignedToId);
            
            return View(version);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVersion(int id, DocumentVersion version)
        {
            if (id != version.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _documentService.UpdateDocumentVersionAsync(version);
                    TempData["SuccessMessage"] = "نسخه سند با موفقیت به‌روزرسانی شد.";
                    return RedirectToAction(nameof(VersionDetails), new { id = version.Id });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (KeyNotFoundException) { return NotFound(); }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"خطا در به‌روزرسانی نسخه: {ex.Message}");
                }
            }
            
            var users = await _userService.GetActiveUsersAsync();
            ViewBag.AssignedToId = new SelectList(users, "Id", "FullName", version.AssignedToId);
            return View(version);
        }
        
        // دانلود فایل‌های پیوست
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            if (id <= 0) return NotFound();

            try
            {
                var attachment = await _documentService.GetAttachmentByIdAsync(id);
                if (attachment == null || string.IsNullOrEmpty(attachment.FilePath))
                    return NotFound("پیوست یافت نشد یا مسیر فایل نامعتبر است.");

                var filePath = attachment.FilePath;
                
                var fileStream = await _fileService.GetFileStreamAsync(filePath);
                
                return File(fileStream, attachment.ContentType ?? "application/octet-stream", attachment.FileName);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error downloading attachment {id}: {ex.Message}");
                return NotFound($"فایل یافت نشد: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading attachment {id}: {ex.Message}");
                return StatusCode(500, "خطا در دانلود فایل.");
            }
        }

        // POST: حذف پیوست
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttachment(int id, int versionId)
        {
            if (id <= 0 || versionId <= 0) return BadRequest("شناسه‌های نامعتبر.");

            try
            {
                await _documentService.DeleteDocumentAttachmentAsync(id);
                TempData["SuccessMessage"] = "پیوست با موفقیت حذف شد.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $"خطا در حذف پیوست: {ex.Message}";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "پیوست مورد نظر یافت نشد.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطای غیرمنتظره در حذف پیوست: {ex.Message}";
            }
            return RedirectToAction(nameof(VersionDetails), new { id = versionId });
        }

        // POST: ارسال/نهایی کردن نسخه
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVersion(int id)
        {
            if (id <= 0) return BadRequest("شناسه نسخه نامعتبر.");
            try
            {
                await _documentService.SendVersionAsync(id);
                TempData["SuccessMessage"] = "نسخه با موفقیت ارسال/نهایی شد. دیگر امکان ویرایش یا تغییر پیوست‌ها وجود ندارد.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = $"خطا در ارسال نسخه: {ex.Message}";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "نسخه مورد نظر یافت نشد.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"خطای غیرمنتظره در ارسال نسخه: {ex.Message}";
            }
            return RedirectToAction(nameof(VersionDetails), new { id = id });
        }

        // POST: افزودن پیوست جدید به نسخه موجود (از طریق مودال در VersionDetails)
        // جدید
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "?")] // دسترسی مناسب؟
        public async Task<IActionResult> AddAttachment(int versionId, int baseDocumentId, IFormFile attachmentFile)
        {
             if (versionId <= 0 || baseDocumentId <= 0)
                 return BadRequest("شناسه‌های نسخه یا سند پایه نامعتبر هستند.");
            
             if (attachmentFile == null || attachmentFile.Length == 0)
            {
                 TempData["ErrorMessage"] = "هیچ فایلی برای آپلود انتخاب نشده است.";
                 return RedirectToAction(nameof(VersionDetails), new { id = versionId });
            }

            try
            {
                 // سرویس DocumentService خودش چک می‌کند که نسخه IsSent نباشد
                 await _documentService.CreateDocumentAttachmentAsync(baseDocumentId, versionId, attachmentFile);
                 TempData["SuccessMessage"] = "پیوست جدید با موفقیت اضافه شد.";
            }
             catch (InvalidOperationException ex) // خطای تلاش برای افزودن به نسخه ارسال شده
            {
                 TempData["ErrorMessage"] = $"خطا در افزودن پیوست: {ex.Message}";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = $"خطا: {ex.Message}";
            }
            catch (Exception ex)
            {
                 TempData["ErrorMessage"] = $"خطای غیرمنتظره در افزودن پیوست: {ex.Message}";
            }

            return RedirectToAction(nameof(VersionDetails), new { id = versionId });
        }
    }
} 