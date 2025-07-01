using BigQuery.Api.Enums;
using System.ComponentModel;

namespace BigQuery.Api.Models
{
    public class QueryOrderByDto
    {
        [DefaultValue(SortDirection.ASC)]
        public string Key { get; set; }

        [DefaultValue("event_timestamp")]
        public string Value { get; set; }
    }
}
