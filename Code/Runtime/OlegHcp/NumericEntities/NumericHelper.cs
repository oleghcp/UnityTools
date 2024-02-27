namespace OlegHcp.NumericEntities
{
    internal static class NumericHelper
    {
        public static float GetRatio(float numerator, float denominator)
        {
            if (denominator == 0f)
                return 1f;

            return numerator / denominator;
        }
    }
}
