using System;
using System.Runtime.CompilerServices;
using Tools;

namespace UnityUtility.Rng
{
    public sealed class DotNetRng : Random, IRng
    {
        public DotNetRng() : base() { }

        public DotNetRng(int seed) : base(seed) { }

        public float NextFloat(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                Errors.MinMax(nameof(minValue), nameof(maxValue));

            return (float)(Sample() * ((double)maxValue - minValue) + minValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextFloat(float maxValue)
        {
            return NextFloat(0f, maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte NextByte()
        {
            return (byte)Next(256);
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr));

            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = (byte)Next(256);
            }
        }
    }
}
