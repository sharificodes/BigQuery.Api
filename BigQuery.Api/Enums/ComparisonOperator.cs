using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BigQuery.Api.Enums
{
    public enum ComparisonOperator
    {
        [DefaultValue("=")]
        Equals,
        [DefaultValue("<>")]
        NotEquals,
        [DefaultValue(">")]
        GreaterThan,
        [DefaultValue("<")]
        LessThan,
        [DefaultValue(">=")]
        GreaterThanOrEquals,
        [DefaultValue("<=")]
        LessThanOrEquals,
        [DefaultValue("LIKE")]
        Like,
        [DefaultValue("NOT LIKE")]
        NotLike,
        [DefaultValue("IN")]
        In,
        [DefaultValue("NOT IN")]
        NotIn,
    }
}
