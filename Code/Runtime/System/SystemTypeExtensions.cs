using System.Runtime.CompilerServices;
using System.Text;
using UU;

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
        public static string GetName(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        /// <summary>
        /// Returns integer value of converted enumeration or non-standard integer enumeration (such as byte, short, etc.)
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInteger(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Indicates whether the specified string is null or an empty string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Indicates whether the specified string is not null and not an empty string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyData(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        ///Indicates whether the specified string is not null, empty, and doesn't consist only of white-space characters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasUsefulData(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Gets the underlying type code of the specified Type
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }

        /// <summary>
        /// Returns true if type is the specified type or subclass of the specified type;
        /// </summary>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this Type type, Type familyType)
        {
            return type.IsSubclassOf(familyType) || type == familyType;
        }
    }
}
