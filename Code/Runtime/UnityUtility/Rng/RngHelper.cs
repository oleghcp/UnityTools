﻿namespace UnityUtility.Rng
{
    internal static class RngHelper
    {
        public static double UintToDouble(uint randomUnsignedInt)
        {
            return randomUnsignedInt / (uint.MaxValue + 1d);
        }

        public static float DoubleToFloat(float minValue, float maxValue, double normalizedRandomDouble)
        {
            return (float)(((double)maxValue - minValue) * normalizedRandomDouble + minValue);
        }
    }
}
