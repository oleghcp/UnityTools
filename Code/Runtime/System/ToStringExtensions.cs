using System.Collections.Generic;

namespace System
{
    public static class ToStringExtensions
    {
        public static string ConcatToString<T>(this IEnumerable<T> self, string separator = "")
        {
            return string.Join(separator, self);
        }

#if UNITY_2021_2_OR_NEWER
        public static string ConcatToString<T>(this IEnumerable<T> self, char separator)
        {
            return string.Join(separator, self);
        }
#endif

        public static string ConcatToString(this IEnumerable<string> self, string separator = "")
        {
            return string.Join(separator, self);
        }

#if UNITY_2021_2_OR_NEWER
        public static string ConcatToString(this IEnumerable<string> self, char separator)
        {
            return string.Join(separator, self);
        }
#endif

        public static string ConcatToString(this object[] self, string separator = "")
        {
            return string.Join(separator, self);
        }

#if UNITY_2021_2_OR_NEWER
        public static string ConcatToString(this object[] self, char separator)
        {
            return string.Join(separator, self);
        }
#endif

        public static string ConcatToString(this string[] self, string separator = "")
        {
            return string.Join(separator, self);
        }

#if UNITY_2021_2_OR_NEWER
        public static string ConcatToString(this string[] self, char separator)
        {
            return string.Join(separator, self);
        }
#endif

        public static string ConcatToString(this string[] self, int startIndex, int count, string separator = "")
        {
            return string.Join(separator, self, startIndex, count);
        }

#if UNITY_2021_2_OR_NEWER
        public static string ConcatToString(this string[] self, int startIndex, int count, char separator)
        {
            return string.Join(separator, self, startIndex, count);
        }
#endif
    }
}
