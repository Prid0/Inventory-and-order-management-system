namespace Pim.Data.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string RequestPath { get; set; }

        public string HTTPMethod { get; set; }

        public string ResponseStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }

        public string ExecutionTime { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
