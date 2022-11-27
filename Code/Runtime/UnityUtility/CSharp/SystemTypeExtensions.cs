using System;
using System.Text;
using UnityUtilityTools;

namespace UnityUtility.CSharp
{
    public static class SystemTypeExtensions
    {
        public static string ToString(this int self, int radix)
        {
            return ConvertUtility.DecimalToStringWithCustomRadix(self, radix);
        }

        public static string ToString(this long self, int radix)
        {
            return ConvertUtility.DecimalToStringWithCustomRadix(self, radix);
        }

        public static string Cut(this StringBuilder self)
        {
            string value = self.ToString();
            self.Clear();
            return value;
        }

        /// <summary>
        /// Retrieves the name of the constant in the specified enumeration that has the specified value.
        /// </summary>        
        public static string GetName(this Enum self)
        {
            return Enum.GetName(self.GetType(), self);
        }

        /// <summary>
        /// Returns the element at the specified index from the end of a string.
        /// </summary>
        public static char FromEnd(this string self, int reverseIndex)
        {
            return self[self.Length - (reverseIndex + 1)];
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
            return self.Replace(" ", string.Empty);
        }

        /// <summary>
        /// Returns the underlying type code of the specified Type.
        /// </summary>
        public static TypeCode GetTypeCode(this Type self)
        {
            return Type.GetTypeCode(self);
        }

        /// <summary>
        /// Returns simplified assembly qualified name.
        /// </summary>
        public static string GetTypeName(this Type self)
        {
            return Helper.SimplifyTypeName(self.AssemblyQualifiedName);
        }

        /// <summary>
        /// Returns default value for the specified Type.
        /// </summary>
        public static object GetDefaultValue(this Type self)
        {
            return Helper.GetDefaultValue(self);
        }

        public static bool IsAssignableTo(this Type self, Type targetType)
        {
            if (targetType == null)
                return false;

            return targetType.IsAssignableFrom(self);
        }

        public static T GetTarget<T>(this WeakReference<T> self) where T : class
        {
            self.TryGetTarget(out var target);
            return target;
        }
    }
}
