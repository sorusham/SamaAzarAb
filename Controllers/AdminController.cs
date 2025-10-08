using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MessageForAzarab.Services.Interface;
using MessageForAzarab.Models;
using Microsoft.AspNetCore.Identity; // Needed for UserManager
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using MessageForAzarab.Services;
using Microsoft.Extensions.Logging;

namespace MessageForAzarab.Controllers
{
    [Authorize(Roles = "Admin")] // Restrict access to Admins only
    public class AdminController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IExcelProcessingService _excelProcessingService; // Inject Excel service
        private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager
        private readonly ILogger<AdminController> _logger; // Add logger

        public AdminController(
            IDepartmentService departmentService,
            IProjectService projectService,
            IExcelProcessingService excelProcessingService, // Add to constructor
            UserManager<ApplicationUser> userManager, // Add to constructor
            ILogger<AdminController> logger) // Add logger to constructor
        {
            _departmentService = departmentService;
            _projectService = projectService;
            _excelProcessingService = excelProcessingService; // Assign injected service
            _userManager = userManager; // Assign injected UserManager
            _logger = logger; // Assign injected logger
        }

        // Action to display the upload form
        [HttpGet]
        public async Task<IActionResult> UploadExcel()
        {
            try
            {
                _logger.LogInformation("درخواست نمایش فرم آپلود اکسل");
                
                // Load Departments and Projects for dropdowns
                ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
                ViewBag.Projects = await _projectService.GetAllProjectsAsync();
                
                _logger.LogInformation($"فرم آپلود اکسل با {ViewBag.Departments.Count} دپارتمان و {ViewBag.Projects.Count} پروژه بارگذاری شد");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در بارگذاری فرم آپلود اکسل");
                TempData["Error"] = "خطا در بارگذاری فرم. لطفاً مجدداً تلاش کنید.";
                return RedirectToAction("Index", "Home");
            }
        }

        // Action to handle the uploaded file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile file, int departmentId, int projectId)
        {
            try
            {
                _logger.LogInformation($"درخواست آپلود فایل اکسل: {file?.FileName}, دپارتمان: {departmentId}, پروژه: {projectId}");
                
                // Validate file
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("فایل آپلود شده خالی یا null است");
                    TempData["Error"] = "لطفاً یک فایل اکسل انتخاب کنید.";
                    return RedirectToAction(nameof(UploadExcel));
                }

                // Validate file size (10MB limit)
                const long maxFileSize = 10 * 1024 * 1024; // 10MB
                if (file.Length > maxFileSize)
                {
                    _logger.LogWarning($"فایل {file.FileName} بیش از حد مجاز است. حجم: {file.Length} بایت");
                    TempData["Error"] = "حجم فایل نمی‌تواند بیش از 10 مگابایت باشد.";
                    return RedirectToAction(nameof(UploadExcel));
                }

                // Validate file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    _logger.LogWarning($"فرمت فایل {file.FileName} مجاز نیست. فرمت: {fileExtension}");
                    TempData["Error"] = "فقط فایل‌های اکسل (.xlsx یا .xls) مجاز هستند.";
                    return RedirectToAction(nameof(UploadExcel));
                }

                // Validate department and project
                if (departmentId <= 0 || projectId <= 0)
                {
                    _logger.LogWarning($"شناسه دپارتمان یا پروژه معتبر نیست. دپارتمان: {departmentId}, پروژه: {projectId}");
                    TempData["Error"] = "لطفاً دپارتمان و پروژه را انتخاب کنید.";
                    return RedirectToAction(nameof(UploadExcel));
                }

                _logger.LogInformation($"شروع پردازش فایل {file.FileName} با حجم {file.Length} بایت");

                using (var stream = file.OpenReadStream())
                {
                    var previewData = await _excelProcessingService.PreviewExcelDataAsync(stream);
                    
                    _logger.LogInformation($"پردازش فایل تکمیل شد. {previewData.Count} رکورد یافت شد");
                    
                    // Store department and project IDs for import
                    TempData["DepartmentId"] = departmentId;
                    TempData["ProjectId"] = projectId;
                    TempData["FileName"] = file.FileName;
                    
                    return View("PreviewExcel", previewData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در پردازش فایل اکسل {file?.FileName}");
                TempData["Error"] = $"خطا در خواندن فایل اکسل: {ex.Message}";
                return RedirectToAction(nameof(UploadExcel));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportSelectedDocuments(List<ExcelPreviewRow> selectedRows)
        {
            try
            {
                _logger.LogInformation($"درخواست وارد کردن {selectedRows?.Count ?? 0} رکورد انتخاب شده");
                
                if (selectedRows == null || !selectedRows.Any())
                {
                    _logger.LogWarning("هیچ رکوردی برای وارد کردن انتخاب نشده است");
                    TempData["Error"] = "هیچ رکوردی برای وارد کردن انتخاب نشده است.";
                    return RedirectToAction("UploadExcel");
                }

                // Filter only selected rows without errors
                var selectedRowsToImport = selectedRows.Where(r => r.IsSelected && !r.HasError).ToList();
                
                if (!selectedRowsToImport.Any())
                {
                    _logger.LogWarning("هیچ رکورد معتبری برای وارد کردن انتخاب نشده است");
                    TempData["Warning"] = "هیچ رکورد معتبری برای وارد کردن انتخاب نشده است.";
                    return RedirectToAction("UploadExcel");
                }

                var departmentId = TempData["DepartmentId"] as int? ?? 0;
                var projectId = TempData["ProjectId"] as int? ?? 0;
                var fileName = TempData["FileName"] as string ?? "نامشخص";
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (departmentId == 0 || projectId == 0)
                {
                    _logger.LogWarning("اطلاعات دپارتمان یا پروژه یافت نشد");
                    TempData["Error"] = "اطلاعات دپارتمان یا پروژه یافت نشد. لطفاً مجدداً فایل را آپلود کنید.";
                    return RedirectToAction("UploadExcel");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("شناسه کاربر یافت نشد");
                    TempData["Error"] = "خطا در احراز هویت کاربر.";
                    return RedirectToAction("UploadExcel");
                }

                _logger.LogInformation($"شروع وارد کردن {selectedRowsToImport.Count} رکورد از فایل {fileName}");

                var result = await _excelProcessingService.ImportSelectedDocumentsAsync(selectedRowsToImport, departmentId, projectId, userId);
                
                _logger.LogInformation($"وارد کردن تکمیل شد. موفق: {result.SuccessCount}, خطا: {result.ErrorCount}");
                
                if (result.SuccessCount > 0)
                {
                    TempData["Success"] = $"با موفقیت {result.SuccessCount} رکورد وارد شد.";
                }
                
                if (result.ErrorCount > 0)
                {
                    TempData["Warning"] = $"{result.ErrorCount} رکورد با خطا مواجه شد.";
                }

                return View("ImportResult", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در وارد کردن داده‌های اکسل");
                TempData["Error"] = $"خطا در وارد کردن داده‌ها: {ex.Message}";
                return RedirectToAction("UploadExcel");
            }
        }

    }
}