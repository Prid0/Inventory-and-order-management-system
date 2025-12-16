namespace Pim.Data.Models
{
    public class ApiRequestLog
    {

        public int Id { get; set; }
        public string RequestPath { get; set; }
        public string HttpMethod { get; set; }
        public int ResponseStatusCode { get; set; }
        public long ElapsedTimeMs { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
