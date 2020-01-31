using System.Runtime.CompilerServices;

namespace System
{
    public static class SystemTypeExtensions
    {
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
        /// Indicates whether the string is not null and not an empty string.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyData(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        ///Indicates whether the string is not null, empty, and doesn't consist only of white-space characters.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasUsefulData(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is(this Type type, TypeCode typeCode)
        {
            return Type.GetTypeCode(type) == typeCode;
        }
    }
}
