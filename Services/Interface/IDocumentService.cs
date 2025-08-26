using MessageForAzarab.Models;
using Microsoft.AspNetCore.Http;

namespace MessageForAzarab.Services.Interface
{
    public interface IDocumentService
    {
        // متدهای مربوط به سند پایه (BaseDocument)
        Task<List<BaseDocument>> GetAllDocumentsAsync();
        Task<List<BaseDocument>> GetActiveDocumentsAsync();
        Task<List<BaseDocument>> GetDocumentsByProjectAsync(int projectId);
        Task<List<BaseDocument>> GetDocumentsByDepartmentAsync(int departmentId);
        Task<BaseDocument?> GetDocumentByIdAsync(int id);
        Task<BaseDocument?> GetDocumentByCodeAsync(string code);
        Task<BaseDocument> CreateDocumentAsync(BaseDocument document);
        Task UpdateDocumentAsync(BaseDocument document);
        Task DeleteDocumentAsync(int id);
        
        // متدهای مربوط به نسخه سند (DocumentVersion)
        Task<List<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId);
        Task<DocumentVersion?> GetVersionByIdAsync(int versionId);
        Task<DocumentVersion> CreateDocumentVersionAsync(DocumentVersion version);
        Task UpdateDocumentVersionAsync(DocumentVersion version);
        Task DeleteDocumentVersionAsync(int versionId);
        Task SendVersionAsync(int versionId);
        
        // متدهای مربوط به تراکنش سند (DocumentTransaction)
        Task<List<DocumentTransaction>> GetTransactionsByVersionIdAsync(int versionId);
        Task<DocumentTransaction?> GetTransactionByIdAsync(int transactionId);
        Task<DocumentTransaction> CreateDocumentTransactionAsync(DocumentTransaction transaction);
        Task UpdateDocumentTransactionAsync(DocumentTransaction transaction);
        Task DeleteDocumentTransactionAsync(int transactionId);
        
        // متدهای مربوط به پیوست سند (DocumentAttachment)
        Task<List<DocumentAttachment>> GetAttachmentsByVersionIdAsync(int versionId);
        Task<DocumentAttachment?> GetAttachmentByIdAsync(int attachmentId);
        Task<DocumentAttachment> CreateDocumentAttachmentAsync(int baseDocumentId, int documentVersionId, IFormFile file);
        Task UpdateDocumentAttachmentAsync(DocumentAttachment attachment);
        Task DeleteDocumentAttachmentAsync(int attachmentId);
        
        // متدهای مربوط به وضعیت سند
        Task<bool> UpdateDocumentStatusAsync(int baseDocumentId, string status);
        Task<bool> UpdateDocumentReviewStageAsync(int baseDocumentId, DocumentReviewStage stage);
        
        // متدهای گزارش‌گیری
        Task<List<BaseDocument>> GetDocumentsWithStatusAsync(string status);
        Task<List<BaseDocument>> GetDocumentsByReviewStageAsync(DocumentReviewStage stage);
    }
} 