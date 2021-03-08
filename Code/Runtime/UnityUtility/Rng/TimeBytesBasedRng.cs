using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
    [Serializable]
    public class TimeBytesBasedRng : IRng
    {
        private ByteGenerator _rng;

        public TimeBytesBasedRng() : this(Environment.TickCount) { }

        public TimeBytesBasedRng(int seed)
        {
            _rng = new ByteGenerator(seed);
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

            return (float)(NextDouble() * ((double)maxValue - minValue) + minValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Next(float maxValue)
        {
            return Next(0f, maxValue);
        }

        public unsafe double NextDouble()
        {
            Span<byte> bytes = stackalloc byte[8];
            NextBytes(bytes);
            ulong rn = *(ulong*)bytes.Ptr;
            rn %= 1000000000000000ul;
            return rn * 0.000000000000001d;
        }

        public byte NextByte()
        {
            return _rng.RandomByte();
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = _rng.RandomByte();
            }
        }

        public void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = _rng.RandomByte();
            }
        }

        private unsafe int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
            {
                byte rn = _rng.RandomByte();
                return rn % (int)length + minValue;
            }
            else if (length <= 65536L)
            {
                Span<byte> bytes = stackalloc byte[2];
                NextBytes(bytes);
                ushort rn = *(ushort*)bytes.Ptr;
                return rn % (int)length + minValue;
            }
            else
            {
                Span<byte> bytes = stackalloc byte[4];
                NextBytes(bytes);
                uint rn = *(uint*)bytes.Ptr;
                return (int)(rn % length + minValue);
            }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ByteGenerator
        {
            private readonly byte MULT;

            private uint _seed;
            private uint _ticks;

            public ByteGenerator(int seed)
            {
                MULT = (byte)((uint)seed % 8 + 2);
                _seed = (uint)seed;
                _ticks = 0;
            }

            public byte RandomByte()
            {
                uint newTicks = (uint)Environment.TickCount;

                if (_ticks < newTicks)
                    _ticks = newTicks;
                else
                    _ticks++;

                _seed = MULT * _seed + _ticks;

                return (byte)(_seed % 256);
            }
        }
    }
}
