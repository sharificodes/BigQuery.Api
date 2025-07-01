using BigQuery.Api.Extensions;
using BigQuery.Api.Models;
using Google;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using SimpleResults;
using System.Data;
using System.Net;
using System.Text;


namespace BigQuery.Api.Services
{
    public class BQ : IBQ
    {
        private readonly BigQueryClient bqClient;
        private readonly string projectId;


        public BQ(BigQueryClient bqClient, IConfiguration configuration)
        {
            this.bqClient = bqClient;
            this.projectId = configuration["project_id"];
        }

        public async Task<Result<List<string>>> GetFieldsAsync(string dataset, string table)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            var query = $"SELECT * FROM `{projectId}.{dataset}.{table}` LIMIT 1";

            var queryResults = await ExecuteQueryAsync(query, null);
            if (queryResults.IsFailed)
                return Result.Failure(queryResults.Errors);

            return queryResults.Data.Schema.Fields.Select(x => x.Name).ToList();
        }

        public async Task<Result<QueryResultDto<List<object>>>> GetRowsAsync(QueryBaseDto dto, string dataset, string table)
        {
            var rows = new List<object>();

            if (dto.Fields == null || dto.Fields.Count() == 0)
                return Result.Failure("هیچ فیلدی مشخص نشده است");

            var (countQuery, parameters) = GenerateCountQuery(dataset, table, dto);
            var queryResults = await ExecuteQueryAsync(countQuery, parameters);
            if (queryResults.IsFailed)
                return Result.Failure(queryResults.Errors);

            var totalRecords = queryResults.Data.Select(x => Convert.ToInt32(x.RawRow.F[0].V)).FirstOrDefault();

            (var query, parameters) = GenerateQuery(dataset, table, dto);

            if (dto.OrderBy != null)
            {
                query.Append($" ORDER BY {dto.OrderBy.Value} {dto.OrderBy.Key}");
            }

            if (dto.PageSize != -1)
                query.Append($" LIMIT {dto.PageSize} OFFSET {(dto.PageNumber - 1) * dto.PageSize}");

            queryResults = await ExecuteQueryAsync(query.ToString(), parameters);
            if (queryResults.IsFailed)
                return Result.Failure(queryResults.Errors);

            foreach (BigQueryRow row in queryResults.Data)
            {
                var cols = GetCols(row, dto.Fields);
                rows.Add(cols);
            }


            return QueryResultDto<List<object>>.ToResult(rows, totalRecords);
        }

        private (StringBuilder query, IEnumerable<BigQueryParameter> bigQueryParameters) GenerateQuery(string dataSet, string table, QueryBaseDto dto)
        {
            var query = new StringBuilder($"SELECT {(dto.RemoveDuplicates && !dto.Fields.Any(x => x.Contains("*")) ? "DISTINCT" : "")} {string.Join(",", dto.Fields)} FROM `{projectId}.{dataSet}.{table}`");
            var bigQueryParameters = GetQueryParameters(dto.Parameters, query);
            return (query, bigQueryParameters);
        }

        private (string query, IEnumerable<BigQueryParameter> bigQueryParameters) GenerateCountQuery(string dataSet, string table, QueryBaseDto dto)
        {
            var (subQuery, bigQueryParameters) = GenerateQuery(dataSet, table, dto);
            var query = $"SELECT COUNT(*) FROM ({subQuery}) AS subquery";
            return (query, bigQueryParameters);
        }

        private static List<BigQueryParameter> GetQueryParameters(IEnumerable<QueryParameterDto> parameters, StringBuilder query)
        {
            var bigQueryParameters = new List<BigQueryParameter>();
            if (parameters != null && parameters.Count() > 0)
            {
                query.Append(" WHERE ");
                var isFirstParameter = true;
                var index = 0;
                foreach (var parameter in parameters)
                {
                    if (!isFirstParameter)
                        query.Append(" AND ");
                    else
                        isFirstParameter = false;

                    query.Append($"{parameter.Key} {parameter.Operator.GetDefaultValueAttribute().Value} @f{index}");
                    var bqParameter = new BigQueryParameter($"f{index++}", null, parameter.Value);
                    bigQueryParameters.Add(bqParameter);
                }
            }

            return bigQueryParameters;
        }

        public async Task<Result<List<object>>> GetRowsByQueryAsync(string query)
        {
            var rows = new List<object>();

            var queryResults = await ExecuteQueryAsync(query.ToString(), null);
            if (queryResults.IsFailed)
                return Result.Failure(queryResults.Errors);

            foreach (BigQueryRow row in queryResults.Data)
            {
                var cols = GetCols(row, row.Schema.Fields.Select(f => f.Name));
                rows.Add(cols);
            }

            return rows;
        }

        private async Task<Result<BigQueryResults>> ExecuteQueryAsync(string query, IEnumerable<BigQueryParameter> parameters)
        {
            var queryOptions = new QueryOptions { UseLegacySql = false };
            var job = await bqClient.CreateQueryJobAsync(sql: query, parameters: parameters, options: queryOptions);

            var errors = job?.Status?.Errors;
            if (errors?.Count > 0)
            {
                return Result.Failure(errors.Select(error => error.Message).ToList());
            }

            try
            {
                job = (await job.PollUntilCompletedAsync()).ThrowOnAnyError();
            }
            catch (GoogleApiException ex)
            {
                return Result.Failure(ex.Error?.Errors?.Select(error => error.Message).ToList());
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            try
            {
                var queryResults = (await bqClient.GetQueryResultsAsync(job.Reference)).ThrowOnAnyError();
                return queryResults;
            }
            catch (GoogleApiException ex)
            {
                return Result.Failure(ex.Error?.Errors?.Select(error => error.Message).ToList());
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        private static Dictionary<string, object> GetCols(BigQueryRow row, IEnumerable<string> fields)
        {
            var cols = new Dictionary<string, object>();

            if (row.Schema.Fields is null)
                return cols;

            if (fields.Any(f => f.Contains("*")))
                fields = row.Schema.Fields.Select(f => f.Name).ToList();

            foreach (var f in fields)
            {
                TableFieldSchema field = null;
                if (f.Contains("."))
                    field = row.Schema.Fields.FirstOrDefault(ff => ff.Name == f.Split('.').Last());
                else
                    field = row.Schema.Fields.FirstOrDefault(ff => ff.Name == f);
                if (field == null)
                    continue;

                cols.Add(f.ToLower(), row[field.Name]);
            }

            return cols;
        }
    }
}
