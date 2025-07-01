using BigQuery.Api.Models;
using SimpleResults;

namespace BigQuery.Api.Services
{
    public interface IBQ
    {
        Task<Result<List<string>>> GetFieldsAsync(string dataset, string table);
        Task<Result<QueryResultDto<List<object>>>> GetRowsAsync(QueryBaseDto dto, string dataset, string table);
        Task<Result<List<object>>> GetRowsByQueryAsync(string query);
    }
}
