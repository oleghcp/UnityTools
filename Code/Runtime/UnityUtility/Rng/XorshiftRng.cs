using System;
using System.Runtime.CompilerServices;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
    [Serializable]
    public class XorshiftRng : IRng
    {
        private uint _a32;
        private ulong _a64;

        public XorshiftRng()
        {
            int seed = RngHelper.GenerateSeed();
            _a32 = (uint)seed;
            _a64 = (ulong)seed;
        }

        public XorshiftRng(int seed)
        {
            if (seed == 0)
                throw new ArgumentException($"Parameter cannot be equal zero.", nameof(seed));

            _a32 = (uint)seed;
            _a64 = (ulong)seed;
        }

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

        public double NextDouble()
        {
            return RngHelper.NextDouble(Xorshift64());
        }

        public byte NextByte()
        {
            return (byte)NextInternal(0, 256);
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)NextInternal(0, 256);
            }
        }

        public void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)NextInternal(0, 256);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float NextInternal(float minValue, float maxValue)
        {
            return (float)(NextDouble() * ((double)maxValue - minValue) + minValue);
        }

        private int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;
            uint rn = Xorshift32();
            return (int)(rn % length + minValue);
        }

        private uint Xorshift32()
        {
            uint x = _a32;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return _a32 = x;
        }

        private ulong Xorshift64()
        {
            ulong x = _a64;
            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            return _a64 = x;
        }
    }
}
