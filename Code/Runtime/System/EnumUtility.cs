using System.Runtime.CompilerServices;

namespace System
{
    public static class Enum<TEnum> where TEnum : Enum
    {
        public static readonly int Count;

        static Enum()
        {
            Count = GetNames().Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(TEnum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] GetValues()
        {
            return Enum.GetValues(typeof(TEnum)) as TEnum[];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(string value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum Parse(string value, bool ignoreCase)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }
    }
}
