using System;

namespace UnityUtility.Rng
{
    internal static class RngHelper
    {
        public static int GenerateSeed()
        {
            int seed = Environment.TickCount;
            if (seed == 0)
                seed++;
            return seed;
        }

        public static double ConvertToDouble(ulong randomLong)
        {
            const ulong repeatLength = 1000000000000000ul;
            const double multiplier = 0.000000000000001d;

            return (randomLong % repeatLength) * multiplier;
        }
    }
}
