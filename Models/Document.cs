using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessageForAzarab.Models
{
    public class BaseDocument
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "کد مدرک آذرآبی الزامی است")]
        [StringLength(50, ErrorMessage = "کد مدرک آذرآبی نمی‌تواند بیش از 50 کاراکتر باشد")]
        [Display(Name = "کد مدرک آذرآبی")]
        public string DocCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "عنوان سند الزامی است")]
        [StringLength(200, ErrorMessage = "عنوان سند نمی‌تواند بیش از 200 کاراکتر باشد")]
        [Display(Name = "عنوان سند")]
        public string Title { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "کد آذرآب نمی‌تواند بیش از 50 کاراکتر باشد")]
        [Display(Name = "کد آذرآب")]
        public string AzarabCode { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "کد مدرک کارفرما نمی‌تواند بیش از 50 کاراکتر باشد")]
        [Display(Name = "کد مدرک کارفرما")]
        public string? ClientDocCode { get; set; }

        [StringLength(50, ErrorMessage = "شماره سند نمی‌تواند بیش از 50 کاراکتر باشد")]
        [Display(Name = "شماره سند")]
        public string? DocNumber { get; set; }

        [StringLength(500, ErrorMessage = "اطلاع‌رسانی نمی‌تواند بیش از 500 کاراکتر باشد")]
        [Display(Name = "اطلاع‌رسانی")]
        public string? Notification { get; set; }

        [StringLength(1000, ErrorMessage = "توضیحات نمی‌تواند بیش از 1000 کاراکتر باشد")]
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "تاریخ سند")]
        public string? DocDate { get; set; }

        [Display(Name = "تاریخ برنامه‌ریزی")]
        public string? PlanDate { get; set; }

        [Display(Name = "اولین ارسال")]
        public string? FirstSubmit { get; set; }

        [Display(Name = "NC")]
        public string? NC { get; set; }

        [Display(Name = "AN")]
        public string? AN { get; set; }

        [Display(Name = "CM")]
        public string? CM { get; set; }

        [Display(Name = "رد شده")]
        public string? Reject { get; set; }

        [Display(Name = "اطلاعات")]
        public string? Information { get; set; }

        [Display(Name = "پیشرفت")]
        public string? Progress { get; set; }

        [Display(Name = "مسئول")]
        public string? Responsible { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "انتخاب دپارتمان الزامی است")]
        [Display(Name = "دپارتمان")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "انتخاب پروژه الزامی است")]
        [Display(Name = "پروژه")]
        public int ProjectId { get; set; }

        [Required]
        [Display(Name = "ایجادکننده")]
        public string CreatorId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "آخرین ویرایش‌کننده")]
        public string LastModifierId { get; set; } = string.Empty;

        [Display(Name = "تاریخ آخرین ویرایش")]
        public DateTime? LastModificationDate { get; set; }

        // Properties from migration
        [Display(Name = "وضعیت")]
        public string Status { get; set; } = "E";  // Default to "E" for active

        [Display(Name = "نسخه فعلی")]
        public int CurrentRevision { get; set; } = 0;  // Default to 0

        [Display(Name = "فعال")]
        public bool IsActive { get; set; } = true;  // Default to true

        [Display(Name = "وضعیت صدور")]
        public DocumentIssueStatus IssueStatus { get; set; } = DocumentIssueStatus.NotIssuable;

        [Display(Name = "مرحله بررسی")]
        public DocumentReviewStage ReviewStage { get; set; } = DocumentReviewStage.Designer;

        [Display(Name = "صحه‌گذار")]
        public string? CheckerId { get; set; }

        [Display(Name = "تصدیق‌کننده")]
        public string? ApproverId { get; set; }

        [Display(Name = "تاریخ صحه‌گذاری")]
        public DateTime? DateChecker { get; set; }

        [Display(Name = "تاریخ تصدیق")]
        public DateTime? DateApprover { get; set; }

        [Display(Name = "معلق")]
        public bool Hold { get; set; } = false;

        [Display(Name = "تاریخ پیش‌بینی")]
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
        [Required(ErrorMessage = "شناسه سند پایه الزامی است")]
        [Display(Name = "سند پایه")]
        public int BaseDocumentId { get; set; } // معادل si_basevdis
        [ForeignKey("BaseDocumentId")]
        public virtual BaseDocument? BaseDocument { get; set; }
        
        // شماره نسخه
        [Required(ErrorMessage = "شماره نسخه الزامی است")]
        [Range(0, int.MaxValue, ErrorMessage = "شماره نسخه باید عدد مثبت باشد")]
        [Display(Name = "شماره نسخه")]
        public int RevisionNumber { get; set; } // معادل si_verd
        
        // ایجادکننده سند
        [Required(ErrorMessage = "ایجادکننده الزامی است")]
        [Display(Name = "ایجادکننده")]
        public string CreatorId { get; set; } = string.Empty;
        [ForeignKey("CreatorId")]
        public virtual ApplicationUser? Creator { get; set; }
        
        // تاریخ ایجاد
        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        
        // تاریخ ارسال
        [Display(Name = "تاریخ ارسال")]
        public DateTime? DateSend { get; set; }
        
        // وضعیت سند
        [Required(ErrorMessage = "وضعیت الزامی است")]
        [Display(Name = "وضعیت")]
        public string Status { get; set; } = "O"; // O = باز، C = بسته شده
        
        // پیشرفت
        [Display(Name = "پیشرفت")]
        [Range(0, 100, ErrorMessage = "پیشرفت باید بین 0 تا 100 باشد")]
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
        
        [Required(ErrorMessage = "نام دپارتمان الزامی است")]
        [StringLength(100, ErrorMessage = "نام دپارتمان نمی‌تواند بیش از 100 کاراکتر باشد")]
        [Display(Name = "نام دپارتمان")]
        public string Name { get; set; } = string.Empty; // معادل deptname
        
        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از 500 کاراکتر باشد")]
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
        [Required(ErrorMessage = "شناسه نسخه سند الزامی است")]
        [Display(Name = "نسخه سند")]
        public int DocumentVersionId { get; set; }
        [ForeignKey("DocumentVersionId")]
        public virtual DocumentVersion DocumentVersion { get; set; } = null!;
        
        // نام فایل
        [Required(ErrorMessage = "نام فایل الزامی است")]
        [StringLength(255, ErrorMessage = "نام فایل نمی‌تواند بیش از 255 کاراکتر باشد")]
        [Display(Name = "نام فایل")]
        public string FileName { get; set; } = string.Empty;
        
        // مسیر فایل
        [Required(ErrorMessage = "مسیر فایل الزامی است")]
        [StringLength(500, ErrorMessage = "مسیر فایل نمی‌تواند بیش از 500 کاراکتر باشد")]
        [Display(Name = "مسیر فایل")]
        public string FilePath { get; set; } = string.Empty;
        
        // حجم فایل
        [Range(0, long.MaxValue, ErrorMessage = "حجم فایل باید عدد مثبت باشد")]
        [Display(Name = "حجم فایل (بایت)")]
        public long FileSize { get; set; }
        
        // نوع فایل
        [Required(ErrorMessage = "نوع فایل الزامی است")]
        [StringLength(100, ErrorMessage = "نوع فایل نمی‌تواند بیش از 100 کاراکتر باشد")]
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

        // محاسبه حجم فایل به صورت خوانا
        [Display(Name = "حجم فایل")]
        public string FileSizeFormatted => FormatFileSize(FileSize);

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
} 