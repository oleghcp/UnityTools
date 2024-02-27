using System.Collections.Generic;

namespace OlegHcp.CSharp
{
    public static class ConcatToStringExtensions
    {
        public static string ConcatToString<T>(this IEnumerable<T> self)
        {
            return string.Concat(self);
        }

        public static string ConcatToString<T>(this IEnumerable<T> self, string separator)
        {
            return string.Join(separator, self);
        }

        public static string ConcatToString<T>(this IEnumerable<T> self, char separator)
        {
#if UNITY_2021_2_OR_NEWER
            return string.Join(separator, self);
#else
            return string.Join($"{separator}", self);
#endif
        }

        public static string ConcatToString(this IEnumerable<string> self)
        {
            return string.Concat(self);
        }

        public static string ConcatToString(this IEnumerable<string> self, string separator)
        {
            return string.Join(separator, self);
        }

        public static string ConcatToString(this IEnumerable<string> self, char separator)
        {
#if UNITY_2021_2_OR_NEWER
            return string.Join(separator, self);
#else
            return string.Join($"{separator}", self);
#endif
        }

        public static string ConcatToString(this object[] self)
        {
            return string.Concat(self);
        }

        public static string ConcatToString(this object[] self, string separator)
        {
            return string.Join(separator, self);
        }

        public static string ConcatToString(this object[] self, char separator)
        {
#if UNITY_2021_2_OR_NEWER
            return string.Join(separator, self);
#else
            return string.Join($"{separator}", self);
#endif
        }

        public static string ConcatToString(this string[] self)
        {
            return string.Concat(self);
        }

        public static string ConcatToString(this string[] self, string separator)
        {
            return string.Join(separator, self);
        }

        public static string ConcatToString(this string[] self, char separator)
        {
#if UNITY_2021_2_OR_NEWER
            return string.Join(separator, self);
#else
            return string.Join($"{separator}", self);
#endif
        }

        public static string ConcatToString(this string[] self, int startIndex, int count)
        {
            return string.Join(string.Empty, self, startIndex, count);
        }

        public static string ConcatToString(this string[] self, int startIndex, int count, string separator)
        {
            return string.Join(separator, self, startIndex, count);
        }

        public static string ConcatToString(this string[] self, int startIndex, int count, char separator)
        {
#if UNITY_2021_2_OR_NEWER
            return string.Join(separator, self, startIndex, count);
#else
            return string.Join($"{separator}", self, startIndex, count);
#endif
        }
    }
}
