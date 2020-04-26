using System;
using static UnityEngine.Random;

namespace UnityUtility.Rng
{
    public sealed class UnityRNG : IRng
    {
        public UnityRNG() { }

        public UnityRNG(int seed)
        {
            InitState(seed);
        }

        public int Next(int minValue, int maxValue)
        {
            return Range(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), nameof(maxValue) + " cannot be negative.");

            return Range(0, maxValue);
        }

        public float NextFloat(float minValue, float maxValue)
        {
            return Range(minValue, maxValue);
        }

        public double NextDouble()
        {
            double res = Range(0, int.MaxValue);
            return res / int.MaxValue;
        }

        public byte NextByte()
        {
            return (byte)Next(0, 256);
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Range(0, 256);
            }
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr), "Pointer cannot be null.");

            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = (byte)Range(0, 256);
            }
        }
    }
}
