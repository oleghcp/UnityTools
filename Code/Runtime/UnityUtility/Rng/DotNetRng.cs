using System;
using System.Runtime.CompilerServices;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
    [Serializable]
    public sealed class DotNetRng : Random, IRng
    {
        public DotNetRng() : base() { }

        public DotNetRng(int seed) : base(seed) { }

        public float Next(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            return (float)(Sample() * ((double)maxValue - minValue) + minValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Next(float maxValue)
        {
            return Next(0f, maxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte NextByte()
        {
            return (byte)base.Next(256);
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr));

            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = (byte)base.Next(256);
            }
        }
    }
}
