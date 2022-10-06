﻿using System;

namespace UnityUtility.Rng
{
    [Serializable]
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

        public override double NextDouble()
        {
            return RngHelper.UintToDouble(Xorshift32());
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
            return (int)(Xorshift32() % length + minValue);
        }

        protected override float NextInternal(float minValue, float maxValue)
        {
            double normalizedRandomDouble = RngHelper.UintToDouble(Xorshift32());
            return RngHelper.DoubleToFloat(minValue, maxValue, normalizedRandomDouble);
        }

        private uint Xorshift32()
        {
            uint x = _num32;
            x ^= x << _a;
            x ^= x >> _b;
            x ^= x << _c;
            return _num32 = x;
        }
    }
}
