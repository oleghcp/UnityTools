using System.Runtime.CompilerServices;

namespace System
{
    public static class Enum<TEnum> where TEnum : Enum
    {
        public static readonly int Count;

        static Enum()
        {
            Count = EnumUtility.GetNames<TEnum>().Length;
        }
    }

    public static class EnumUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetNames<TEnum>() where TEnum : Enum
        {
            return Enum.GetNames(typeof(TEnum));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum[] GetValues<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)) as TEnum[];
        }
    }
}
