using System;
using System.Collections.Generic;
using System.Text;

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

        public static string ConcatToString<T>(this Span<T> self) where T : unmanaged
        {
            return ConcatToString(self, 0, self.Length, builder => { });
        }

        public static string ConcatToString<T>(this Span<T> self, string separator) where T : unmanaged
        {
            return ConcatToString(self, 0, self.Length, builder => builder.Append(separator));
        }

        public static string ConcatToString<T>(this Span<T> self, char separator) where T : unmanaged
        {
            return ConcatToString(self, 0, self.Length, builder => builder.Append(separator));
        }

        public static string ConcatToString<T>(this Span<T> self, int startIndex, int count) where T : unmanaged
        {
            return ConcatToString(self, startIndex, count, builder => { });
        }

        public static string ConcatToString<T>(this Span<T> self, int startIndex, int count, string separator) where T : unmanaged
        {
            return ConcatToString(self, startIndex, count, builder => builder.Append(separator));
        }

        public static string ConcatToString<T>(this Span<T> self, int startIndex, int count, char separator) where T : unmanaged
        {
            return ConcatToString(self, startIndex, count, builder => builder.Append(separator));
        }

        private static string ConcatToString<T>(this Span<T> span, int startIndex, int count, Action<StringBuilder> append) where T : unmanaged
        {
            if ((uint)startIndex >= (uint)span.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count == 0)
                return string.Empty;

            if (count == 1)
                return span[startIndex].ToString();

            StringBuilder builder = new StringBuilder();

            for (int i = startIndex; i < count + startIndex; i++)
            {
                builder.Append(span[i].ToString());
                append(builder);
            }

            return builder.ToString();
        }
    }
}
