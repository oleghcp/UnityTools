using System;
using OlegHcp.Tools;

namespace OlegHcp.CSharp
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the element at the specified index from the end of a string.
        /// </summary>
        public static char FromEnd(this string self, int index)
        {
            return self[self.Length - (index + 1)];
        }

        /// <summary>
        /// Indicates whether the specified string is null or an empty string.
        /// </summary>
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        /// <summary>
        /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }

        /// <summary>
        /// Indicates whether the specified string is not null and not an empty string.
        /// </summary>
        public static bool HasAnyData(this string self)
        {
            return !string.IsNullOrEmpty(self);
        }

        /// <summary>
        ///Indicates whether the specified string is not null, empty, and doesn't consist only of white-space characters.
        /// </summary>
        public static bool HasUsefulData(this string self)
        {
            return !string.IsNullOrWhiteSpace(self);
        }

        public static string RemoveWhiteSpaces(this string self)
        {
            return self.Replace(Helper.Space, string.Empty);
        }

        public static char GetRandomChar(this string self, IRng generator)
        {
            if (self.Length == 0)
                throw new InvalidOperationException("String is empty.");

            return self[generator.Next(self.Length)];
        }

        public static char GetRandomChar(this string self)
        {
            return GetRandomChar(self, RandomNumberGenerator.Default);
        }
    }
}
