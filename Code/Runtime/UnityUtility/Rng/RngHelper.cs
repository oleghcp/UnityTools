using System.Runtime.CompilerServices;

namespace UnityUtility.Rng
{
    internal static class RngHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetNormalizedDouble(uint randomUnsignedInt)
        {
            return randomUnsignedInt / (uint.MaxValue + 0.00001d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetNormalizedFloat(ushort randomUnsignedShort)
        {
            return randomUnsignedShort / (ushort.MaxValue + 0.01f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomFloat(float minValue, float maxValue, double normalizedRandomDouble)
        {
            return (float)(((double)maxValue - minValue) * normalizedRandomDouble + minValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RandomFloat(float minValue, float maxValue, float normalizedRandomFloat)
        {
            return (maxValue - minValue) * normalizedRandomFloat + minValue;
        }
    }
}
