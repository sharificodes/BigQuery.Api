using BigQuery.Api.Enums;
using System.ComponentModel;

namespace BigQuery.Api.Extensions
{
    public static class EnumExtension
    {
        public static DefaultValueAttribute GetDefaultValueAttribute(this ComparisonOperator comparisonOperator)
        {
            var fieldInfo = comparisonOperator.GetType().GetField(comparisonOperator.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0];
            }

            return null;
        }
    }
}
