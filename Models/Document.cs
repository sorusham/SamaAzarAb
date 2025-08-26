using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageForAzarab.Models
{
    public class BaseDocument
    {
        [Key]
        public int Id { get; set; }
        public string DocCode { get; set; }
        public string Title { get; set; }
        public string AzarabCode { get; set; }
        public string ClientDocCode { get; set; }
        public string? DocNumber { get; set; }
        public string? Notification { get; set; }
        public string? Description { get; set; }
        public string? DocDate { get; set; }
        public string? PlanDate { get; set; }
        public string? FirstSubmit { get; set; }
        public string? NC { get; set; }
        public string? AN { get; set; }
        public string? CM { get; set; }
        public string? Reject { get; set; }
        public string? Information { get; set; }
        public string? Progress { get; set; }
        public string? Responsible { get; set; }
        public DateTime CreationDate { get; set; }
        public int DepartmentId { get; set; }
        public int ProjectId { get; set; }
        public string CreatorId { get; set; }
        public string LastModifierId { get; set; }
        public DateTime? LastModificationDate { get; set; }
        // Properties from migration
        public string Status { get; set; } = "E";  // Default to "E" for active
        public int CurrentRevision { get; set; } = 0;  // Default to 0
        public bool IsActive { get; set; } = true;  // Default to true
        public DocumentIssueStatus IssueStatus { get; set; } = DocumentIssueStatus.NotIssuable;
        public DocumentReviewStage ReviewStage { get; set; } = DocumentReviewStage.Designer;
        public string? CheckerId { get; set; }
        public string? ApproverId { get; set; }
        public DateTime? DateChecker { get; set; }
        public DateTime? DateApprover { get; set; }
        public bool Hold { get; set; } = false;
        public DateTime? ForecastDate { get; set; }
        
        [ForeignKey("CheckerId")]
        public ApplicationUser? Checker { get; set; }     
        
        [ForeignKey("ApproverId")]
        public ApplicationUser? Approver { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        [ForeignKey("CreatorId")]
        public virtual ApplicationUser Creator { get; set; }
        [ForeignKey("LastModifierId")]
        public virtual ApplicationUser LastModifier { get; set; }
        // لیست تراکنش‌های این سند
        public virtual ICollection<DocumentVersion> DocumentVersions { get; set; } = new List<DocumentVersion>();
    }
    
    // این مدل معادل جدول prjdcc_vdis در دیتابیس قدیمی است
    public class DocumentVersion
    {
        [Key]
        public int Id { get; set; } // معادل Si در دیتابیس قدیمی
        
        // ارتباط با سند پایه
        [Required]
        public int BaseDocumentId { get; set; } // معادل si_basevdis
        [ForeignKey("BaseDocumentId")]
        public virtual BaseDocument? BaseDocument { get; set; }
        
        // شماره نسخه
        [Display(Name = "شماره نسخه")]
        public int RevisionNumber { get; set; } // معادل si_verd
        
        // ایجادکننده سند
        [Display(Name = "ایجادکننده")]
        public string CreatorId { get; set; } 
        [ForeignKey("CreatorId")]
        public virtual ApplicationUser? Creator { get; set; }
        
        // تاریخ ایجاد
        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        // تاریخ ارسال
        [Display(Name = "تاریخ ارسال")]
        public DateTime? DateSend { get; set; }
        
        // وضعیت سند
        [Display(Name = "وضعیت")]
        public string Status { get; set; } = "O"; // O = باز، C = بسته شده
        
        // پیشرفت
        [Display(Name = "پیشرفت")]
        [Range(0, 100)]
        public int Progress { get; set; } = 0;
        
        // تخصیص به
        [Display(Name = "تخصیص به")]
        public string? AssignedToId { get; set; }
        [ForeignKey("AssignedToId")]
        public virtual ApplicationUser? AssignedTo { get; set; }
        
        // آیا مخفی است؟
        [Display(Name = "مخفی")]
        public bool IsHidden { get; set; } = false;

        // آیا نسخه ارسال/نهایی شده است؟
        [Display(Name = "ارسال شده")]
        public bool IsSent { get; set; } = false;
        
        // لیست تراکنش‌های مربوط به این نسخه
        public virtual ICollection<DocumentTransaction> Transactions { get; set; } = new List<DocumentTransaction>();
        
        // لیست فایل‌های پیوست
        public virtual ICollection<DocumentAttachment> Attachments { get; set; } = new List<DocumentAttachment>();

        [Display(Name = "نسخه قبلی")]
        public int? PreviousVersionId { get; set; }
        [ForeignKey("PreviousVersionId")]
        public virtual DocumentVersion? PreviousVersion { get; set; }
    }
    
    // این مدل معادل جدول prjdcc_trans در دیتابیس قدیمی است
    public class DocumentTransaction
    {
        [Key]
        public int Id { get; set; } // معادل transno
        
        // ارتباط با نسخه سند
        [Required]
        public int DocumentVersionId { get; set; } // معادل si_vdis
        [ForeignKey("DocumentVersionId")]
        public virtual DocumentVersion DocumentVersion { get; set; } = null!;
        
        // ارتباط با پروژه
        public int? ProjectId { get; set; } // معادل siproject
        
        // تاریخ تراکنش
        [Display(Name = "تاریخ تراکنش")]
        public DateTime TransactionDate { get; set; } = DateTime.Now; // معادل transdate
        
        // شماره تراکنش خارجی
        [Display(Name = "شماره تراکنش خارجی")]
        public string? ExternalTransactionNumber { get; set; } // معادل vtransno
        
        // وضعیت پاسخ
        [Display(Name = "وضعیت پاسخ")]
        public DocumentStatus? ResponseStatus { get; set; } // معادل vpoiNo
        
        // تاریخ پاسخ
        [Display(Name = "تاریخ پاسخ")]
        public DateTime? ReplyDate { get; set; } // معادل replydate
        
        // برگه نظرات
        [Display(Name = "برگه نظرات")]
        public string? CommentSheet { get; set; }
        
        // پاسخ به نظرات
        [Display(Name = "پاسخ به نظرات")]
        public string? ResponseToComments { get; set; } // معادل R_CommentSheet
        
        // شماره نامه
        [Display(Name = "شماره نامه")]
        public string? LetterNumber { get; set; } // معادل LetterNo
        
        // شماره خلاصه
        [Display(Name = "شماره خلاصه")]
        public string? RecapNumber { get; set; } // معادل RecapNo
        
        // وضعیت آزراب
        [Display(Name = "وضعیت آزراب")]
        public string? AzarabStatus { get; set; } // معادل StatusAzarab
        
        // تاریخ داده
        [Display(Name = "تاریخ داده")]
        public DateTime? DataDate { get; set; } // معادل vdata_date
    }
    
    // این مدل معادل جدول prjdcc_dept در دیتابیس قدیمی است
    public class Department
    {
        [Key]
        public int Id { get; set; } // معادل Si
        
        [Required]
        [Display(Name = "نام دپارتمان")]
        public string Name { get; set; } = string.Empty; // معادل deptname
        
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
        
        // تاریخ ایجاد
        [Display(Name = "تاریخ ایجاد")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        
        // لیست اسناد این دپارتمان
        public virtual ICollection<BaseDocument> Documents { get; set; } = new List<BaseDocument>();
        
        // لیست پروژه‌های مرتبط با این دپارتمان
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        
        // ارتباط با کاربران این دپارتمان
        public virtual ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
    }
    
    // این مدل برای نگهداری فایل‌های پیوست سند است
    public class DocumentAttachment
    {
        [Key]
        public int Id { get; set; }
        
        //ارتباط با رویژن   
        [Required]
        public int DocumentVersionId { get; set; }
        [ForeignKey("DocumentVersionId")]
        public virtual DocumentVersion DocumentVersion { get; set; } = null!;
        
        // نام فایل
        [Required]
        [Display(Name = "نام فایل")]
        public string FileName { get; set; } = string.Empty;
        
        // مسیر فایل
        [Required]
        public string FilePath { get; set; } = string.Empty;
        
        // حجم فایل
        [Display(Name = "حجم فایل")]
        public long FileSize { get; set; }
        
        // نوع فایل
        [Display(Name = "نوع فایل")]
        public string ContentType { get; set; } = string.Empty;
        
        // تاریخ آپلود
        [Display(Name = "تاریخ آپلود")]
        public DateTime UploadDate { get; set; } = DateTime.Now;
        
        // آپلود کننده
        [Display(Name = "آپلود کننده")]
        public string? UploaderId { get; set; }
        [ForeignKey("UploaderId")]
        public virtual ApplicationUser? Uploader { get; set; }
    }
} 