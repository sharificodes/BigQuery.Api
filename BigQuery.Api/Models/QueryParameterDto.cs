using BigQuery.Api.Enums;
using System.ComponentModel;

namespace BigQuery.Api.Models
{
    public class QueryParameterDto
    {
        public QueryParameterDto()
        {

        }
        public QueryParameterDto(string key, ComparisonOperator comparisonOperator, string value)
        {
            Key = key;
            Operator = comparisonOperator;
            Value = value;
        }

        public QueryParameterDto(string key, ComparisonOperator comparisonOperator, string valueMin, string valueMax)
        {
            Key = key;
            Operator = comparisonOperator;
        }

        [DefaultValue("platform")]
        public string Key { get; set; }

        [DefaultValue(ComparisonOperator.Equals)]
        public ComparisonOperator Operator { get; set; }

        [DefaultValue("ANDROID")]
        public string Value { get; set; } 
    }
}
