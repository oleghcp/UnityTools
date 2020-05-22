using System.Runtime.CompilerServices;
using System.Text;
using UnityUtility;

namespace System
{
    public static class SystemTypeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this int self, int radix)
        {
            return ConvertUtility.DecimalToStringWithCustomRadix(self, radix);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this long self, int radix)
        {
            return ConvertUtility.DecimalToStringWithCustomRadix(self, radix);
        }

        public static string Cut(this StringBuilder self)
        {
            var value = self.ToString();
            self.Clear();
            return value;
        }

        /// <summary>
        /// Retrieves the name of the constant in the specified enumeration that has the specified value.
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetName(this Enum self)
        {
            return Enum.GetName(self.GetType(), self);
        }

        /// <summary>
        /// Returns integer value of converted enumeration or non-standard integer enumeration (such as byte, short, etc.)
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInteger(this Enum self)
        {
            return Convert.ToInt32(self);
        }

        /// <summary>
        /// Indicates whether the specified string is null or an empty string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        /// <summary>
        /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string self)
        {
            return string.IsNullOrWhiteSpace(self);
        }

        /// <summary>
        /// Indicates whether the specified string is not null and not an empty string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyData(this string self)
        {
            return !string.IsNullOrEmpty(self);
        }

        /// <summary>
        ///Indicates whether the specified string is not null, empty, and doesn't consist only of white-space characters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasUsefulData(this string self)
        {
            return !string.IsNullOrWhiteSpace(self);
        }

        /// <summary>
        /// Gets the underlying type code of the specified Type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeCode GetTypeCode(this Type self)
        {
            return Type.GetTypeCode(self);
        }

        /// <summary>
        /// Gets default value for the specified Type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetDefaultValue(this Type self)
        {
            return Helper.GetDefaultValue(self);
        }

        /// <summary>
        /// Returns true if type is the specified type or subclass of the specified type;
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this Type self, Type familyType)
        {
            return self.IsSubclassOf(familyType) || self == familyType;
        }
    }
}
