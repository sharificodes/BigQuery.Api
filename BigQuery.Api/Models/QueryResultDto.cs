namespace BigQuery.Api.Models
{
    public class QueryResultDto<T>
    {
        public T Value { get; set; }
        public int TotalRecords { get; set; }

        public static QueryResultDto<T> ToResult(T value, int totalRecords)
        {
            return new QueryResultDto<T>
            {
                TotalRecords = totalRecords,
                Value = value
            };
        }
    }
}
