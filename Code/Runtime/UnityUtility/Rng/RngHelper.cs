using System.Runtime.CompilerServices;

namespace UnityUtility.Rng
{
    internal static class RngHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetNormalizedDouble(uint randomUnsignedInt)
        {
            return randomUnsignedInt / (double)uint.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetNormalizedFloat(ushort randomUnsignedShort)
        {
            return randomUnsignedShort / (float)ushort.MaxValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomFloat(float minValue, float maxValue, float normalizedRandomFloat)
        {
            return (maxValue - minValue) * normalizedRandomFloat + minValue;
        }
    }
}
