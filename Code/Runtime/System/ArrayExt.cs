using System.Runtime.CompilerServices;

namespace System
{
    public static class ArrayExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(T[] array)
        {
            return array == null || array.Length == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAnyData<T>(T[] array)
        {
            return array != null && array.Length > 0;
        }
    }
}
