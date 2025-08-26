namespace MessageForAzarab.Models
{
    public class ExcelPreviewRow
    {
        public int RowNumber { get; set; }
        public string Title { get; set; }
        public string DocCode { get; set; }
        public string AzarabCode { get; set; }
        public string ClientDocCode { get; set; }
        public string DocNumber { get; set; }
        public string Notification { get; set; }
        public string DocDate { get; set; }
        public string PlanDate { get; set; }
        public string FirstSubmit { get; set; }
        public string NC { get; set; }
        public string AN { get; set; }
        public string CM { get; set; }
        public string Reject { get; set; }
        public string Information { get; set; }
        public string Progress { get; set; }
        public string Responsible { get; set; }
        public bool IsDuplicate { get; set; }
        public bool IsSelected { get; set; } = true;
        public bool HasError { get; set; }
        public string ValidationMessage { get; set; }
    }

    public class ImportResult
    {
        public bool IsSuccess { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
} 