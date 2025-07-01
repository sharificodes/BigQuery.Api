using BigQuery.Api.Constants;
using BigQuery.Api.Enums;
using BigQuery.Api.Filters;
using BigQuery.Api.Models;
using BigQuery.Api.Services;
using Microsoft.AspNetCore.Mvc;
using SimpleResults;

namespace BigQuery.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/firebase-crashlytic")]
    [ApiVersion("1.0")]
    [ApiKeyActionFilter]
    public class FirebaseCrashlyticController : Controller
    {
        private readonly IBQ _bigQuery;
        private const string DATA_SET = DataSets.FirebaseCrashlytics;
        private const string TABLE = Tables.app_ANDROID;

        public FirebaseCrashlyticController(IBQ bigQuery)
        {
            _bigQuery = bigQuery;
        }

        [HttpGet("fields")]
        public async Task<Result<List<string>>> GetFirebaseCrashlyticFields()
        {
            var result = await _bigQuery.GetFieldsAsync(DATA_SET, TABLE);
            if (result.IsFailed)
                return result;

            result.Data.RemoveAll(x => x.Contains("event_timestamp"));
            return result;
        }

        [HttpPost("query")]
        public async Task<Result<QueryResultDto<List<object>>>> GetFirebaseCrashlytics([FromBody] FirebaseCrashlyticQueryDto dto)
        {
            if (dto == null)
                return Result.Invalid("آرگومان های ورودی را به درستی وارد کنید");

            if (dto.Parameters.Count > 0)
            {
                dto.Parameters.RemoveAll(x => x.Key.Contains("event_timestamp"));
            }

            if (dto.FromDate.HasValue)
            {
                string fromDate = dto.FromDate.Value.ToString("yyyy-MM-dd");
                dto.Parameters.Add(new QueryParameterDto("DATE(event_timestamp)", ComparisonOperator.GreaterThanOrEquals, fromDate));
            }

            if (dto.ToDate.HasValue)
            {
                string toDate = dto.ToDate.Value.ToString("yyyy-MM-dd");
                dto.Parameters.Add(new QueryParameterDto("DATE(event_timestamp)", ComparisonOperator.LessThanOrEquals, toDate));
            }

            if (!string.IsNullOrEmpty(dto.UserId))
                dto.Parameters.Add(new QueryParameterDto("user.id", ComparisonOperator.Equals, dto.UserId));

            var result = await _bigQuery.GetRowsAsync(dto, DATA_SET, TABLE);

            return result;
        }

        [HttpGet("query")]
        public async Task<Result<List<object>>> GetFirebaseCrashlyticsByQuery([FromQuery] QueryDto dto)
        {
            if (dto == null)
                return Result.Invalid("آرگومان های ورودی را به درستی وارد کنید");

            if (string.IsNullOrEmpty(dto.Query))
                return Result.Invalid("آرگومان کوئری خالی است");

            var result = await _bigQuery.GetRowsByQueryAsync(dto.Query);

            return result;
        }
    }
}
