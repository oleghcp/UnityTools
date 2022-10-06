namespace UnityUtility.Rng
{
    internal static class RngHelper
    {
        public static double UintToDouble(uint randomUnsignedInt)
        {
            return (double)randomUnsignedInt / uint.MaxValue;
        }

        public static float DoubleToFloat(float minValue, float maxValue, double normalizedRandomDouble)
        {
            return (maxValue - minValue) * (float)normalizedRandomDouble + minValue;
        }
    }
}
