namespace CandyCustomizer.Runtime
{
    using System;
    using System.Reflection;
    using CandyCustomizer.Metadata;

    internal static class CandyNameResolver
    {
        public static string Resolve(object candy)
        {
            if (candy is null)
                return string.Empty;

            string propertyValue = TryReadCandyProperty(candy);

            if (!string.IsNullOrWhiteSpace(propertyValue))
                return Normalize(propertyValue);

            return Normalize(candy.GetType().Name);
        }

        private static string TryReadCandyProperty(object candy)
        {
            foreach (string propertyName in new[] { "Kind", "KindId", "KindID", "CandyKindId", "CandyKindID", "Id", "ID" })
            {
                PropertyInfo property = candy.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (property is null || property.GetIndexParameters().Length != 0)
                    continue;

                object value = property.GetValue(candy);

                if (value is not null)
                    return value.ToString();
            }

            return string.Empty;
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string candyName = CandyMetadataRegistry.FindCandyName(value);

            if (!string.IsNullOrWhiteSpace(candyName))
                return candyName;

            return value.Replace("Haunted", string.Empty).Replace("Candy", string.Empty).Trim();
        }
    }
}
