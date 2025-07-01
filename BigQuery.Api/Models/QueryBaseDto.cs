using BigQuery.Api.Enums;
using System.ComponentModel;

namespace BigQuery.Api.Models
{
    public abstract class QueryBaseDto
    {
        public QueryBaseDto()
        {
            PageSize = 10;
            Fields = Enumerable.Empty<string>();
            Parameters = new List<QueryParameterDto>();
        }

        public bool RemoveDuplicates { get; set; }

        [DefaultValue(new string[] { "*" })]
        public IEnumerable<string> Fields { get; set; }

        public List<QueryParameterDto> Parameters { set; get; }

        public QueryOrderByDto OrderBy { get; set; }

        [DefaultValue(1)]
        public int PageNumber { get; set; }

        [DefaultValue(10)]
        public int PageSize { get; set; }
    }
}
