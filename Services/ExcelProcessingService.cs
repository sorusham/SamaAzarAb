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

        private readonly ApplicationDbContext _context;
        private readonly PersianCalendar _persianCalendar;

        public ExcelProcessingService(ApplicationDbContext context)
        {
            _context = context;
            _persianCalendar = new PersianCalendar();
        }

        public async Task<List<ExcelPreviewRow>> PreviewExcelDataAsync(Stream excelStream)
        {
            var previewRows = new List<ExcelPreviewRow>();
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);

            var lastRow = worksheet.LastRowUsed().RowNumber();
            var existingDocCodes = await _context.BaseDocuments
                .Select(d => new { d.DocCode, d.AzarabCode })
                .ToListAsync();

            for (int row = DATA_START_ROW; row <= lastRow; row++)
            {
                try
                {
                    var docName = worksheet.Cell(row, COL_IDX_DOC_NAME).GetString()?.Trim() ?? string.Empty;
                    var azarabCode = worksheet.Cell(row, COL_IDX_AZARAB).GetString()?.Trim() ?? string.Empty;
                    var vendorCode = worksheet.Cell(row, COL_IDX_VENDOR).GetString()?.Trim() ?? string.Empty;
                    
                    if (string.IsNullOrWhiteSpace(docName) && string.IsNullOrWhiteSpace(azarabCode) && string.IsNullOrWhiteSpace(vendorCode))
                        continue;

                    var previewRow = new ExcelPreviewRow
                    {
                        RowNumber = row,
                        Title = docName,
                        AzarabCode = azarabCode,
                        ClientDocCode = vendorCode,
                        DocNumber = worksheet.Cell(row, COL_IDX_DOC_NUMBER).GetString()?.Trim() ?? string.Empty,
                        Notification = worksheet.Cell(row, COL_IDX_NOTIFICATION).GetString()?.Trim() ?? string.Empty,
                        DocDate = worksheet.Cell(row, COL_IDX_DOC_DATE).GetString()?.Trim() ?? string.Empty,
                        PlanDate = worksheet.Cell(row, COL_IDX_PLAN_DATE).GetString()?.Trim() ?? string.Empty,
                        FirstSubmit = worksheet.Cell(row, COL_IDX_FIRST_SUBMIT).GetString()?.Trim() ?? string.Empty,
                        NC = worksheet.Cell(row, COL_IDX_NC).GetString()?.Trim() ?? string.Empty,
                        AN = worksheet.Cell(row, COL_IDX_AN).GetString()?.Trim() ?? string.Empty,
                        CM = worksheet.Cell(row, COL_IDX_CM).GetString()?.Trim() ?? string.Empty,
                        Reject = worksheet.Cell(row, COL_IDX_REJECT).GetString()?.Trim() ?? string.Empty,
                        Information = worksheet.Cell(row, COL_IDX_INFORMATION).GetString()?.Trim() ?? string.Empty,
                        Progress = worksheet.Cell(row, COL_IDX_PROGRESS).GetString()?.Trim() ?? string.Empty,
                        Responsible = worksheet.Cell(row, COL_IDX_RESPONSIBLE).GetString()?.Trim() ?? string.Empty,
                        DocCode = azarabCode,
                        IsDuplicate = existingDocCodes.Any(d => d.DocCode == azarabCode || d.AzarabCode == azarabCode),
                        IsSelected = true,
                        HasError = false
                    };

                    ValidateRow(previewRow, existingDocCodes);
                    previewRows.Add(previewRow);
                }
                catch (Exception ex)
                {
                    previewRows.Add(new ExcelPreviewRow
                    {
                        RowNumber = row,
                        DocCode = $"Row {row}",
                        Title = "Error",
                        ValidationMessage = $"خطا در خواندن سطر {row}: {ex.Message}",
                        HasError = true,
                        IsSelected = false
                    });
                }
            }

            return previewRows;
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
                foreach (var row in selectedRows.Where(r => r.IsSelected && !r.HasError))
                {
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
                        // Fix for CS0029: Cannot implicitly convert type 'int' to 'string'
                        // The issue is that the `CreatorId` property in `DocumentVersion` is of type `string`,
                        // but the code is assigning an `int` value (`creatorUserId`) to it.
                        // To fix this, we need to convert `creatorUserId` to a string.

                        CreatorId = creatorUserId.ToString(),
                        
                        Status = "O"
                    };

                    newBaseDocument.DocumentVersions.Add(newVersion);
                    documentsToAdd.Add(newBaseDocument);
                }

                // Save all documents in one transaction
                _context.BaseDocuments.AddRange(documentsToAdd);
                await _context.SaveChangesAsync();
                result.SuccessCount = documentsToAdd.Count;
                result.ErrorCount = selectedRows.Count(r => r.HasError || !r.IsSelected);
                if (result.ErrorCount > 0)
                {
                    result.ErrorMessages.Add($"تعداد {result.ErrorCount} ردیف به دلیل خطا یا عدم انتخاب وارد نشدند.");
                }
            }
            catch (Exception ex)
            {
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