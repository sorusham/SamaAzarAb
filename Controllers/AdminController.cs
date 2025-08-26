using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MessageForAzarab.Services.Interface;
using MessageForAzarab.Models;
using Microsoft.AspNetCore.Identity; // Needed for UserManager
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using MessageForAzarab.Services;

namespace MessageForAzarab.Controllers
{
    [Authorize(Roles = "Admin")] // Restrict access to Admins only
    public class AdminController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IExcelProcessingService _excelProcessingService; // Inject Excel service
        private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager

        public AdminController(
            IDepartmentService departmentService,
            IProjectService projectService,
            IExcelProcessingService excelProcessingService, // Add to constructor
            UserManager<ApplicationUser> userManager) // Add to constructor
        {
            _departmentService = departmentService;
            _projectService = projectService;
            _excelProcessingService = excelProcessingService; // Assign injected service
            _userManager = userManager; // Assign injected UserManager
        }

        // Action to display the upload form
        [HttpGet]
        public async Task<IActionResult> UploadExcel()
        {
            // Load Departments and Projects for dropdowns
            ViewBag.Departments = await _departmentService.GetAllDepartmentsAsync();
            ViewBag.Projects = await _projectService.GetAllProjectsAsync();
            return View();
        }

        // Action to handle the uploaded file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile file, int departmentId, int projectId)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "لطفاً یک فایل اکسل انتخاب کنید.";
                return RedirectToAction(nameof(UploadExcel));
            }

            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            {
                TempData["Error"] = "فقط فایل‌های اکسل (.xlsx یا .xls) مجاز هستند.";
                return RedirectToAction(nameof(UploadExcel));
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var previewData = await _excelProcessingService.PreviewExcelDataAsync(stream);
                    TempData["DepartmentId"] = departmentId;
                    TempData["ProjectId"] = projectId;
                    return View("PreviewExcel", previewData);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطا در خواندن فایل اکسل: {ex.Message}";
                return RedirectToAction(nameof(UploadExcel));
            }
        }

        //    [HttpPost]
        //    public async Task<IActionResult> ImportSelectedDocuments(List<ExcelPreviewRow> selectedRows)
        //    {
        //        if (!selectedRows.Any())
        //        {
        //            TempData["Error"] = "هیچ رکوردی برای وارد کردن انتخاب نشده است.";
        //            return RedirectToAction(nameof(UploadExcel));
        //        }

        //        var departmentId = TempData["DepartmentId"] as int? ?? 0;
        //        var projectId = TempData["ProjectId"] as int? ?? 0;
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //        try
        //        {
        //            var result = await _excelProcessingService.ImportSelectedDocumentsAsync(selectedRows.Select(r => new Models.ExcelPreviewRow
        //            {
        //                SheetName = r.SheetName,
        //                RowNumber = r.RowNumber,
        //                DocCode = r.DocCode,
        //                DocName = r.DocName,
        //                AzarabCode = r.AzarabCode,
        //                DocDate = r.DocDate,
        //                POI = r.POI,
        //                Discipline = r.Discipline,
        //                DocType = r.DocType,
        //                AsBuild = r.AsBuild,
        //                Comment = r.Comment,
        //                Weight = r.Weight,
        //                AFC = r.AFC,
        //                Plan = r.Plan,
        //                Remin = r.Remin,
        //                Responsible = r.Responsible,
        //                FinalBook = r.FinalBook,
        //                IsSelected = r.IsSelected,
        //                ValidationMessage = r.ValidationMessage,
        //                HasError = r.HasError
        //            }).ToList(), departmentId, projectId, userId);

        //            if (result.IsSuccess)
        //            {
        //                TempData["Success"] = $"تعداد {result.SuccessCount} رکورد با موفقیت وارد شدند.";
        //                if (result.ErrorCount > 0)
        //                {
        //                    TempData["Warning"] = string.Join("\n", result.ErrorMessages);
        //                }
        //            }
        //            else
        //            {
        //                TempData["Error"] = string.Join("\n", result.ErrorMessages);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Error"] = $"خطا در وارد کردن اطلاعات: {ex.Message}";
        //        }

        //        return RedirectToAction(nameof(UploadExcel));
        //    }
        //}

        public class ExcelPreviewRow
        {
            public string SheetName { get; set; }
            public int RowNumber { get; set; }
            public string DocCode { get; set; }
            public string DocName { get; set; }
            public string AzarabCode { get; set; }
            public string DocDate { get; set; }
            public string POI { get; set; }
            public string Discipline { get; set; }
            public string DocType { get; set; }
            public string AsBuild { get; set; }
            public string Comment { get; set; }
            public string Weight { get; set; }
            public string AFC { get; set; }
            public string Plan { get; set; }
            public string Remin { get; set; }
            public string Responsible { get; set; }
            public string FinalBook { get; set; }
            public bool IsSelected { get; set; } = true;
            public string ValidationMessage { get; set; }
            public bool HasError { get; set; }
        }
    }
}