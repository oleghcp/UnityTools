using System;

namespace UnityUtility.Rng
{
    internal static class RngHelper
    {
        public static double NextDouble(ulong randomLong)
        {
            const ulong repeatLength = 1000000000000000ul;
            const double multiplier = 0.000000000000001d;

            return (randomLong % repeatLength) * multiplier;
        }

        public static int GenerateSeed()
        {
            int seed = Environment.TickCount;
            if (seed == 0)
                seed++;
            return seed;
        }
    }
}
