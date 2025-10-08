namespace MessageForAzarab.Models
{
    public class ExcelPreviewRow
    {
        public string SheetName { get; set; } = string.Empty;
        public int RowNumber { get; set; }
        public string DocCode { get; set; } = string.Empty;
        public string DocName { get; set; } = string.Empty;
        public string AzarabCode { get; set; } = string.Empty;
        public string DocDate { get; set; } = string.Empty;
        public string POI { get; set; } = string.Empty;
        public string Discipline { get; set; } = string.Empty;
        public string DocType { get; set; } = string.Empty;
        public string AsBuild { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
        public string AFC { get; set; } = string.Empty;
        public string Plan { get; set; } = string.Empty;
        public string Remin { get; set; } = string.Empty;
        public string Responsible { get; set; } = string.Empty;
        public string FinalBook { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = true;
        public string ValidationMessage { get; set; } = string.Empty;
        public bool HasError { get; set; }
        
        // Additional properties for compatibility
        public string Title { get; set; } = string.Empty;
        public string ClientDocCode { get; set; } = string.Empty;
        public string DocNumber { get; set; } = string.Empty;
        public string Notification { get; set; } = string.Empty;
        public string PlanDate { get; set; } = string.Empty;
        public string FirstSubmit { get; set; } = string.Empty;
        public string NC { get; set; } = string.Empty;
        public string AN { get; set; } = string.Empty;
        public string CM { get; set; } = string.Empty;
        public string Reject { get; set; } = string.Empty;
        public string Information { get; set; } = string.Empty;
        public string Progress { get; set; } = string.Empty;
        public bool IsDuplicate { get; set; }
    }

    public class ImportResult
    {
        public bool IsSuccess { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
} 