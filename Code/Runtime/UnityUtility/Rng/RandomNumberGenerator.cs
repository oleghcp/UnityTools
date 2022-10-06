using System;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
    [Serializable]
    public abstract class RandomNumberGenerator : IRng
    {
        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            return NextInternal(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw Errors.NegativeParameter(nameof(maxValue));

            return NextInternal(0, maxValue);
        }

        public float Next(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            return NextInternal(minValue, maxValue);
        }

        public float Next(float maxValue)
        {
            if (maxValue < 0f)
                throw Errors.NegativeParameter(nameof(maxValue));

            return NextInternal(0f, maxValue);
        }

        public abstract double NextDouble();

        public abstract void NextBytes(byte[] buffer);
        public abstract void NextBytes(Span<byte> buffer);

        protected abstract int NextInternal(int minValue, int maxValue);
        protected abstract float NextInternal(float minValue, float maxValue);
    }
}
