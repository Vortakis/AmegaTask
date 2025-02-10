namespace FinPriceFeed.Core.Model
{
    public class PaginatedResult<T>
    {
        public int TotalCount { get; set; }

        public int Count { get; set; }

        public List<T> Result { get; set; }

        public PaginatedResult(List<T> result, int totalCount, int count)
        {
            Result = result;
            TotalCount = totalCount;
            Count = count;
        }
    }
}
