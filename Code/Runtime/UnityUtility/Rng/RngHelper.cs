namespace UnityUtility.Rng
{
    internal static class RngHelper
    {
        public static double UlongToDouble(ulong randomUnsignedLong)
        {
            const ulong repeatLength = 1000000000000000ul;
            const double multiplier = 0.000000000000001d;

            return (randomUnsignedLong % repeatLength) * multiplier;
        }

        public static float DoubleToFloat(float minValue, float maxValue, double normalizedRandomDouble)
        {
            return (maxValue - minValue) * (float)normalizedRandomDouble + minValue;
        }
    }
}
