using MessageForAzarab.Models;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MessageForAzarab.Services.Interface
{
    public interface IExcelProcessingService 
    {
        /// <summary>
        /// Imports BaseDocument and their first DocumentVersion from an Excel file stream.
        /// Assumes each row in Excel corresponds to a new BaseDocument and its initial version.
        /// </summary>
        /// <param name="stream">The stream containing the Excel file (.xlsx).</param>
        /// <param name="departmentId">The Department ID to assign to all imported documents.</param>
        /// <param name="projectId">The Project ID to assign to all imported documents.</param>
        /// <param name="creatorUserId">The ID of the user performing the upload.</param>
        /// <returns>An ImportResult object indicating success/failure and counts.</returns>
        Task<ImportResult> ImportBaseDocumentsFromExcelAsync(Stream stream, int departmentId, int projectId, string creatorUserId);
        Task<List<ExcelPreviewRow>> PreviewExcelDataAsync(Stream stream);
        Task<ImportResult> ImportSelectedDocumentsAsync(List<ExcelPreviewRow> selectedRows, int departmentId, int projectId, string creatorUserId);
    }
} 