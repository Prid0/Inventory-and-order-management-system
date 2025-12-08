namespace Pim.Utility
{
    public class PagedResult<T>
    {

        public PagedResult(IEnumerable<T> data, int totalRecord)
        {
            Data = data;
            TotalRecord = totalRecord;
        }

        public int TotalRecord { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
