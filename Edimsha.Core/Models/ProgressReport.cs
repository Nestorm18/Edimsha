namespace Edimsha.Core.Models
{
    public enum ReportType
    {
        Percent,
        Message,
        Finalizated
    }
    
    public class ProgressReport
    {
        public ReportType ReportType { get; set; }
        public object Data { get; set; }
    }
}