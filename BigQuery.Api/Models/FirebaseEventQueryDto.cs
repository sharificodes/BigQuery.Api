using BigQuery.Api.Constants;
using System.ComponentModel;

namespace BigQuery.Api.Models
{
    public class FirebaseEventQueryDto : QueryBaseDto
    {
        [DefaultValue(null)]
        public string UserId { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }
    }
}
