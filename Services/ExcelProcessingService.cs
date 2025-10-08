using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using MessageForAzarab.Data;
using MessageForAzarab.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MessageForAzarab.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MessageForAzarab.Services
{
    public class ExcelProcessingService : IExcelProcessingService
    {
        private const int DATA_START_ROW = 4;
        private const int COL_IDX_DOC_NAME = 3; // Column D
        private const int COL_IDX_AZARAB = 5;   // Column F
        private const int COL_IDX_VENDOR = 6;   // Column G
        private const int COL_IDX_DOC_NUMBER = 7; // Column H
        private const int COL_IDX_NOTIFICATION = 8; // Column I
        private const int COL_IDX_DOC_DATE = 9; // Column J
        private const int COL_IDX_PLAN_DATE = 10; // Column K
        private const int COL_IDX_FIRST_SUBMIT = 11; // Column L
        private const int COL_IDX_NC = 12; // Column M
        private const int COL_IDX_AN = 13; // Column N
        private const int COL_IDX_CM = 14; // Column O
        private const int COL_IDX_REJECT = 15; // Column P
        private const int COL_IDX_INFORMATION = 16; // Column Q
        private const int COL_IDX_PROGRESS = 17; // Column R
        private const int COL_IDX_RESPONSIBLE = 18; // Column S

        private const int BATCH_SIZE = 100;
        private const int MAX_DOC_CODE_LENGTH = 50;
        private const int MAX_TITLE_LENGTH = 200;
        private const int MAX_NOTIFICATION_LENGTH = 500;
        private const int MAX_INFORMATION_LENGTH = 1000;
        private const int MAX_ROWS_PER_PROCESSING = 1000;
        private const int CACHE_EXPIRY_MINUTES = 30;

        private readonly ApplicationDbContext _context;
        private readonly PersianCalendar _persianCalendar;
        private readonly ILogger<ExcelProcessingService> _logger;

        public ExcelProcessingService(ApplicationDbContext context, ILogger<ExcelProcessingService> logger)
        {
            _context = context;
            _persianCalendar = new PersianCalendar();
            _logger = logger;
        }

        public async Task<List<ExcelPreviewRow>> PreviewExcelDataAsync(Stream excelStream)
        {
            try
            {
                _logger.LogInformation("شروع پردازش فایل اکسل");
                
                var previewRows = new List<ExcelPreviewRow>();
                using var workbook = new XLWorkbook(excelStream);
                
                if (workbook.Worksheets.Count == 0)
                {
                    _logger.LogWarning("فایل اکسل هیچ شیتی ندارد");
                    return previewRows;
                }

                var worksheet = workbook.Worksheet(1);
                var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
                
                if (lastRow < DATA_START_ROW)
                {
                    _logger.LogWarning("فایل اکسل داده‌ای برای پردازش ندارد");
                    return previewRows;
                }

                var totalRows = lastRow - DATA_START_ROW + 1;
                _logger.LogInformation($"پردازش {totalRows} سطر از فایل اکسل");

                // Validate row limit for performance
                if (totalRows > MAX_ROWS_PER_PROCESSING)
                {
                    _logger.LogWarning($"تعداد سطرها ({totalRows}) بیش از حد مجاز ({MAX_ROWS_PER_PROCESSING}) است");
                    throw new InvalidOperationException($"تعداد سطرها نمی‌تواند بیش از {MAX_ROWS_PER_PROCESSING} باشد. لطفاً فایل را به بخش‌های کوچک‌تر تقسیم کنید.");
                }

                // Cache existing document codes for performance
                var existingDocCodes = await _context.BaseDocuments
                    .AsNoTracking()
                    .Select(d => new { d.DocCode, d.AzarabCode })
                    .ToListAsync();

                int processedRows = 0;
                int errorRows = 0;

                // Process rows in batches for better performance
                for (int row = DATA_START_ROW; row <= lastRow; row++)
                {
                    try
                    {
                        var docName = GetCellValue(worksheet, row, COL_IDX_DOC_NAME);
                        var azarabCode = GetCellValue(worksheet, row, COL_IDX_AZARAB);
                        var vendorCode = GetCellValue(worksheet, row, COL_IDX_VENDOR);
                        
                        if (string.IsNullOrWhiteSpace(docName) && string.IsNullOrWhiteSpace(azarabCode) && string.IsNullOrWhiteSpace(vendorCode))
                            continue;

                        var previewRow = new ExcelPreviewRow
                        {
                            SheetName = worksheet.Name,
                            RowNumber = row,
                            Title = docName,
                            DocName = docName,
                            AzarabCode = azarabCode,
                            ClientDocCode = vendorCode,
                            DocNumber = GetCellValue(worksheet, row, COL_IDX_DOC_NUMBER),
                            Notification = GetCellValue(worksheet, row, COL_IDX_NOTIFICATION),
                            DocDate = GetCellValue(worksheet, row, COL_IDX_DOC_DATE),
                            PlanDate = GetCellValue(worksheet, row, COL_IDX_PLAN_DATE),
                            FirstSubmit = GetCellValue(worksheet, row, COL_IDX_FIRST_SUBMIT),
                            NC = GetCellValue(worksheet, row, COL_IDX_NC),
                            AN = GetCellValue(worksheet, row, COL_IDX_AN),
                            CM = GetCellValue(worksheet, row, COL_IDX_CM),
                            Reject = GetCellValue(worksheet, row, COL_IDX_REJECT),
                            Information = GetCellValue(worksheet, row, COL_IDX_INFORMATION),
                            Progress = GetCellValue(worksheet, row, COL_IDX_PROGRESS),
                            Responsible = GetCellValue(worksheet, row, COL_IDX_RESPONSIBLE),
                            DocCode = azarabCode,
                            // Map additional properties for compatibility
                            POI = GetCellValue(worksheet, row, COL_IDX_NOTIFICATION), // Map to available column
                            Discipline = GetCellValue(worksheet, row, COL_IDX_DOC_NUMBER), // Map to available column
                            DocType = GetCellValue(worksheet, row, COL_IDX_DOC_DATE), // Map to available column
                            AsBuild = GetCellValue(worksheet, row, COL_IDX_PLAN_DATE), // Map to available column
                            Comment = GetCellValue(worksheet, row, COL_IDX_INFORMATION), // Map to available column
                            Weight = GetCellValue(worksheet, row, COL_IDX_PROGRESS), // Map to available column
                            AFC = GetCellValue(worksheet, row, COL_IDX_NC), // Map to available column
                            Plan = GetCellValue(worksheet, row, COL_IDX_PLAN_DATE), // Map to available column
                            Remin = GetCellValue(worksheet, row, COL_IDX_FIRST_SUBMIT), // Map to available column
                            FinalBook = GetCellValue(worksheet, row, COL_IDX_AN), // Map to available column
                            IsDuplicate = existingDocCodes.Any(d => d.DocCode == azarabCode || d.AzarabCode == azarabCode),
                            IsSelected = true,
                            HasError = false
                        };

                        ValidateRow(previewRow, existingDocCodes);
                        previewRows.Add(previewRow);
                        processedRows++;

                        if (previewRow.HasError)
                            errorRows++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"خطا در پردازش سطر {row}");
                        previewRows.Add(new ExcelPreviewRow
                        {
                            RowNumber = row,
                            DocCode = $"Row {row}",
                            Title = "Error",
                            ValidationMessage = $"خطا در خواندن سطر {row}: {ex.Message}",
                            HasError = true,
                            IsSelected = false
                        });
                        errorRows++;
                    }
                }

                _logger.LogInformation($"پردازش فایل اکسل تکمیل شد. {processedRows} سطر پردازش شد، {errorRows} سطر دارای خطا");
                return previewRows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای کلی در پردازش فایل اکسل");
                throw new InvalidOperationException($"خطا در پردازش فایل اکسل: {ex.Message}", ex);
            }
        }

        private string GetCellValue(IXLWorksheet worksheet, int row, int column)
        {
            try
            {
                var cell = worksheet.Cell(row, column);
                if (cell.IsEmpty())
                    return string.Empty;

                // Handle different cell types
                if (cell.DataType == XLDataType.DateTime)
                {
                    return cell.GetDateTime().ToString("yyyy/MM/dd");
                }
                else if (cell.DataType == XLDataType.Number)
                {
                    return cell.GetDouble().ToString();
                }
                else
                {
                    return cell.GetString()?.Trim() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"خطا در خواندن سلول ({row}, {column})");
                return string.Empty;
            }
        }

        private void ValidateRow(ExcelPreviewRow row, IEnumerable<dynamic> existingDocCodes)
        {
            if (string.IsNullOrWhiteSpace(row.DocCode))
            {
                row.ValidationMessage = "کد سند الزامی است";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(row.Title))
            {
                row.ValidationMessage = "عنوان سند الزامی است";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (row.DocCode.Length > MAX_DOC_CODE_LENGTH)
            {
                row.ValidationMessage = $"کد سند نمی‌تواند بیشتر از {MAX_DOC_CODE_LENGTH} کاراکتر باشد";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (row.Title.Length > MAX_TITLE_LENGTH)
            {
                row.ValidationMessage = $"عنوان سند نمی‌تواند بیشتر از {MAX_TITLE_LENGTH} کاراکتر باشد";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (row.Notification?.Length > MAX_NOTIFICATION_LENGTH)
            {
                row.ValidationMessage = $"اعلان نمی‌تواند بیشتر از {MAX_NOTIFICATION_LENGTH} کاراکتر باشد";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (row.Information?.Length > MAX_INFORMATION_LENGTH)
            {
                row.ValidationMessage = $"توضیحات نمی‌تواند بیشتر از {MAX_INFORMATION_LENGTH} کاراکتر باشد";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (!string.IsNullOrWhiteSpace(row.DocDate) && !IsValidDate(row.DocDate))
            {
                row.ValidationMessage = "فرمت تاریخ سند معتبر نیست";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (!string.IsNullOrWhiteSpace(row.PlanDate) && !IsValidDate(row.PlanDate))
            {
                row.ValidationMessage = "فرمت تاریخ برنامه معتبر نیست";
                row.HasError = true;
                row.IsSelected = false;
                return;
            }

            if (!string.IsNullOrWhiteSpace(row.Progress))
            {
                if (!int.TryParse(row.Progress, out int progress) || progress < 0 || progress > 100)
                {
                    row.ValidationMessage = "درصد پیشرفت باید عددی بین 0 تا 100 باشد";
                    row.HasError = true;
                    row.IsSelected = false;
                    return;
                }
            }

            if (row.IsDuplicate)
            {
                row.ValidationMessage = "این کد سند قبلاً در سیستم ثبت شده است";
                row.HasError = true;
                row.IsSelected = false;
            }
        }

        private bool IsValidDate(string dateStr)
        {
            if (string.IsNullOrWhiteSpace(dateStr))
                return true;

            // Standard Gregorian formats
            string[] gregorianFormats = { "yyyy/MM/dd", "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy" };
            if (DateTime.TryParseExact(dateStr, gregorianFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                return true;

            // Persian date format (yyyy/mm/dd)
            if (TryParsePersianDate(dateStr, out _))
                return true;

            return false;
        }

        private bool TryParsePersianDate(string persianDate, out DateTime? gregorianDate)
        {
            gregorianDate = null;

            try
            {
                var dateParts = persianDate.Split('/', '-');
                if (dateParts.Length != 3)
                    return false;

                if (!int.TryParse(dateParts[0], out int year) ||
                    !int.TryParse(dateParts[1], out int month) ||
                    !int.TryParse(dateParts[2], out int day))
                    return false;

                if (year < 1300 || year > 1500 || month < 1 || month > 12 || day < 1 || day > 31)
                    return false;

                gregorianDate = _persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ImportResult> ImportSelectedDocumentsAsync(List<ExcelPreviewRow> selectedRows, int departmentId, int projectId, string creatorUserId)
        {
            var result = new ImportResult { IsSuccess = true };
            var documentsToAdd = new List<BaseDocument>();

            try
            {
                _logger.LogInformation($"شروع وارد کردن {selectedRows.Count} رکورد انتخاب شده");

                // Validate input parameters
                if (selectedRows == null || !selectedRows.Any())
                {
                    result.IsSuccess = false;
                    result.ErrorMessages.Add("هیچ رکوردی برای وارد کردن انتخاب نشده است.");
                    return result;
                }

                if (departmentId <= 0 || projectId <= 0)
                {
                    result.IsSuccess = false;
                    result.ErrorMessages.Add("شناسه دپارتمان یا پروژه معتبر نیست.");
                    return result;
                }

                if (string.IsNullOrWhiteSpace(creatorUserId))
                {
                    result.IsSuccess = false;
                    result.ErrorMessages.Add("شناسه کاربر ایجادکننده معتبر نیست.");
                    return result;
                }

                // Verify department and project exist
                var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId);
                var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);

                if (!departmentExists)
                {
                    result.IsSuccess = false;
                    result.ErrorMessages.Add("دپارتمان انتخاب شده یافت نشد.");
                    return result;
                }

                if (!projectExists)
                {
                    result.IsSuccess = false;
                    result.ErrorMessages.Add("پروژه انتخاب شده یافت نشد.");
                    return result;
                }

                // Filter valid rows
                var validRows = selectedRows.Where(r => r.IsSelected && !r.HasError).ToList();
                _logger.LogInformation($"{validRows.Count} رکورد معتبر برای وارد کردن یافت شد");

                // Pre-check for duplicates to avoid database calls in loop
                var docCodesToCheck = validRows.Select(r => r.DocCode).Concat(validRows.Select(r => r.AzarabCode)).Distinct().ToList();
                var existingDocs = await _context.BaseDocuments
                    .AsNoTracking()
                    .Where(d => docCodesToCheck.Contains(d.DocCode) || docCodesToCheck.Contains(d.AzarabCode))
                    .Select(d => new { d.DocCode, d.AzarabCode })
                    .ToListAsync();

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Process in batches for better performance
                    var batches = validRows.Chunk(BATCH_SIZE);
                    
                    foreach (var batch in batches)
                    {
                        foreach (var row in batch)
                        {
                            try
                            {
                                // Check for duplicate DocCode using cached data
                                var isDuplicate = existingDocs.Any(d => d.DocCode == row.DocCode || d.AzarabCode == row.AzarabCode);
                                
                                if (isDuplicate)
                                {
                                    result.ErrorMessages.Add($"کد سند {row.DocCode} قبلاً در سیستم ثبت شده است (ردیف {row.RowNumber})");
                                    continue;
                                }

                                var newBaseDocument = new BaseDocument
                                {
                                    Title = row.Title,
                                    DocCode = row.DocCode,
                                    AzarabCode = row.AzarabCode,
                                    ClientDocCode = row.ClientDocCode,
                                    DocNumber = row.DocNumber,
                                    Notification = row.Notification,
                                    DocDate = row.DocDate,
                                    PlanDate = row.PlanDate,
                                    FirstSubmit = row.FirstSubmit,
                                    NC = row.NC,
                                    AN = row.AN,
                                    CM = row.CM,
                                    Reject = row.Reject,
                                    Information = row.Information,
                                    Progress = row.Progress,
                                    Responsible = row.Responsible,
                                    DepartmentId = departmentId,
                                    ProjectId = projectId,
                                    CreatorId = creatorUserId,
                                    CreationDate = DateTime.Now,
                                    LastModificationDate = DateTime.Now,
                                    Status = "E",
                                    CurrentRevision = 0,
                                    IsActive = true,
                                    IssueStatus = DocumentIssueStatus.NotIssuable,
                                    ReviewStage = DocumentReviewStage.Designer
                                };

                                // Create initial version
                                var newVersion = new DocumentVersion
                                {
                                    BaseDocument = newBaseDocument,
                                    RevisionNumber = 0,
                                    CreationDate = DateTime.Now,
                                    CreatorId = creatorUserId,
                                    Status = "O"
                                };

                                newBaseDocument.DocumentVersions.Add(newVersion);
                                documentsToAdd.Add(newBaseDocument);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"خطا در ایجاد سند برای ردیف {row.RowNumber}");
                                result.ErrorMessages.Add($"خطا در ایجاد سند برای ردیف {row.RowNumber}: {ex.Message}");
                            }
                        }
                    }

                    if (documentsToAdd.Any())
                    {
                        // Save in batches for better performance
                        var documentBatches = documentsToAdd.Chunk(BATCH_SIZE);
                        foreach (var docBatch in documentBatches)
                        {
                            _context.BaseDocuments.AddRange(docBatch);
                            await _context.SaveChangesAsync();
                        }
                        
                        await transaction.CommitAsync();
                        
                        result.SuccessCount = documentsToAdd.Count;
                        _logger.LogInformation($"{result.SuccessCount} سند با موفقیت وارد شد");
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        result.IsSuccess = false;
                        result.ErrorMessages.Add("هیچ سند معتبری برای وارد کردن یافت نشد.");
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "خطا در تراکنش وارد کردن اسناد");
                    throw;
                }

                result.ErrorCount = selectedRows.Count(r => r.HasError || !r.IsSelected);
                if (result.ErrorCount > 0)
                {
                    result.ErrorMessages.Add($"تعداد {result.ErrorCount} ردیف به دلیل خطا یا عدم انتخاب وارد نشدند.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای کلی در وارد کردن اطلاعات");
                result.IsSuccess = false;
                result.ErrorMessages.Add($"خطای کلی در وارد کردن اطلاعات: {ex.Message}");
            }

            return result;
        }

        public async Task<ImportResult> ImportBaseDocumentsFromExcelAsync(Stream stream, int departmentId, int projectId, string creatorUserId)
        {
            var previewData = await PreviewExcelDataAsync(stream);
            return await ImportSelectedDocumentsAsync(previewData, departmentId, projectId, creatorUserId);
        }
    }
} 