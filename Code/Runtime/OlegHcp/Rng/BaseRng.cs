using System;
using OlegHcp.Mathematics;
using OlegHcp.Tools;

namespace OlegHcp.Rng
{
    public class BaseRng : Random, IRng
    {
        public BaseRng() : base() { }

        public BaseRng(int seed) : base(seed) { }

        public float Next(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw ThrowErrors.MinMax(nameof(minValue), nameof(maxValue));

            return MathUtility.LerpUnclamped(minValue, maxValue, (float)Sample());
        }

        public float Next(float maxValue)
        {
            if (maxValue < 0f)
                throw ThrowErrors.NegativeParameter(nameof(maxValue));

            return MathUtility.LerpUnclamped(0f, maxValue, (float)Sample());
        }
    }
}
