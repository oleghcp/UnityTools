using System;

namespace UnityUtility.Rng
{
    [Serializable]
    public class XorshiftRng : RandomNumberGenerator
    {
        private uint _a32;
        private ulong _a64;

        public XorshiftRng()
        {
            int seed = Environment.TickCount;
            Initialize(seed == 0 ? seed + 1 : seed);
        }

        public XorshiftRng(int seed)
        {
            if (seed == 0)
                throw new ArgumentException($"Parameter cannot be equal zero.", nameof(seed));

            Initialize(seed);
        }

        public override double NextDouble()
        {
            return RngHelper.UlongToDouble(Xorshift64());
        }

        public override void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)NextInternal(0, 256);
            }
        }

        public override void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)NextInternal(0, 256);
            }
        }

        protected override float NextInternal(float minValue, float maxValue)
        {
            double randomDouble = RngHelper.UlongToDouble(Xorshift64());
            return RngHelper.DoubleToFloat(minValue, maxValue, randomDouble);
        }

        protected override int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;
            uint rn = Xorshift32();
            return (int)(rn % length + minValue);
        }

        private void Initialize(int seed)
        {
            _a32 = (uint)seed;
            _a64 = (ulong)seed;
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
