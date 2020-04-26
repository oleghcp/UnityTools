using System;

namespace UnityUtility.RNGenerators
{
    public sealed class DotNetRNG : Random, IRng
    {
        public DotNetRNG() : base() { }

        public DotNetRNG(int seed) : base(seed) { }

        public float NextFloat(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} cannot be more than {nameof(maxValue)}.");

            return (float)(Sample() * ((double)maxValue - minValue) + minValue);
        }

        public byte NextByte()
        {
            return (byte)Next(256);
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr), "Pointer cannot be null.");

            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = (byte)Next(256);
            }
        }
    }
}
