using System;

namespace WebSocket4UWP.Internal
{
    internal static class StringExtensions
    {
        public static bool StartsAndEndsWith(this string s, string value)
        {
            return s.StartsAndEndsWith(value, StringComparison.CurrentCulture);
        }

        public static bool StartsAndEndsWith(this string s, string value, StringComparison comparisonType)
        {
            return s.StartsWith(value, comparisonType) && s.EndsWith(value, comparisonType);
        }
    }
}
