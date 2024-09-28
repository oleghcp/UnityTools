using System;
using OlegHcp.Mathematics;

namespace OlegHcp.Rng
{
#if UNITY
    [Serializable]
#endif
    public class XorshiftRng : RandomNumberGenerator
    {
        private readonly int _a;
        private readonly int _b;
        private readonly int _c;
        private uint _num32;

        public XorshiftRng() : this(Environment.TickCount)
        {

        }

        public XorshiftRng(int seed) : this(seed, 13, 17, 5)
        {

        }

        public XorshiftRng(int seed, int a, int b, int c)
        {
            if (seed == 0)
                throw new ArgumentException($"Parameter cannot be equal zero.", nameof(seed));

            _num32 = (uint)seed;

            _a = a;
            _b = b;
            _c = c;
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

        protected override int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;
            Xorshift32();
            return (int)(_num32 % length + minValue);
        }

        protected override float NextInternal(float minValue, float maxValue)
        {
            Xorshift32();
            float normalizedRandomFloat = _num32 / (float)uint.MaxValue;
            return MathUtility.LerpUnclamped(minValue, maxValue, normalizedRandomFloat);
        }

        private void Xorshift32()
        {
            uint x = _num32;
            x ^= x << _a;
            x ^= x >> _b;
            x ^= x << _c;
            _num32 = x;
        }
    }
}
