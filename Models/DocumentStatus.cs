using System.ComponentModel.DataAnnotations;

namespace MessageForAzarab.Models
{
    // وضعیت صدور سند
    public enum DocumentIssueStatus
    {
        [Display(Name = "غیرقابل صدور")]
        NotIssuable,
        
        [Display(Name = "در حال صدور")]
        Issuing,
        
        [Display(Name = "صادر شده")]
        Issued,
        
        [Display(Name = "باطل شده")]
        Revoked
    }

    // مرحله بررسی سند
    public enum DocumentReviewStage
    {
        [Display(Name = "Desinger")]
        Designer,
        
        [Display(Name = "Checker")]
        Checker,
        
        [Display(Name = "Approver")]
        Approver,
        
        [Display(Name = "Vendor")]
        Vendor
    }

    // سطح محرمانگی سند
    public enum DocumentSecurityLevel
    {
        [Display(Name = "عادی")]
        Normal,
        
        [Display(Name = "محرمانه")]
        Confidential,
        
        [Display(Name = "خیلی محرمانه")]
        HighlyConfidential,
        
        [Display(Name = "سری")]
        Secret,
        
        [Display(Name = "به کلی سری")]
        TopSecret
    }

    // وضعیت سند در تراکنش‌ها
    public enum DocumentStatus
    {
        [Display(Name = "No Comment")]
        NoComment,
        
        [Display(Name = "With Comment")]
        WithComment,
        
        [Display(Name = "Approved")]
        Approved,
        
        [Display(Name = "Rejected")]
        Rejected,
        
        [Display(Name = "Approved With Note")]
        ApprovedWithComment,
        
    }
} 