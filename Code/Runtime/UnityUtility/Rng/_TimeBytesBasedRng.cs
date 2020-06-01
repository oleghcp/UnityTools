using System;
using System.Runtime.InteropServices;

namespace UnityUtility.Rng
{
    public class TimeBytesBasedRng : IRng
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ByteGenerator
        {
            private readonly byte MULT;

            private uint m_seed;
            private uint m_ticks;

            public ByteGenerator(int seed)
            {
                MULT = (byte)((uint)seed % 8 + 2);
                m_seed = (uint)seed;
                m_ticks = 0;
            }

            public byte RandomByte()
            {
                uint newTicks = (uint)Environment.TickCount;

                if (m_ticks < newTicks)
                    m_ticks = newTicks;
                else
                    m_ticks++;

                m_seed = MULT * m_seed + m_ticks;

                return (byte)(m_seed % 256);
            }
        }

        private ByteGenerator m_rng;

        public TimeBytesBasedRng() : this(Environment.TickCount) { }

        public TimeBytesBasedRng(int seed)
        {
            m_rng = new ByteGenerator(seed);
        }

        public unsafe int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} cannot be more than {nameof(maxValue)}.");

            return f_next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), nameof(maxValue) + " cannot be negative.");

            return f_next(0, maxValue);
        }

        public float NextFloat(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} cannot be more than {nameof(maxValue)}.");

            return (float)(NextDouble() * ((double)maxValue - minValue) + minValue);
        }

        public unsafe double NextDouble()
        {
            byte* bytes = stackalloc byte[8];
            f_bytes(bytes, 8);
            ulong rn = *(ulong*)bytes;
            rn %= 1000000000000000ul;
            return rn * 0.000000000000001d;
        }

        public byte NextByte()
        {
            return m_rng.RandomByte();
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = m_rng.RandomByte();
            }
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr), "Pointer cannot be null.");

            f_bytes(arrayPtr, length);
        }

        // -- //

        private unsafe void f_bytes(byte* arrayPtr, int length)
        {
            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = m_rng.RandomByte();
            }
        }

        private unsafe int f_next(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
            {
                byte rn = m_rng.RandomByte();
                return rn % (int)length + minValue;
            }
            else if (length <= 65536L)
            {
                byte* bytes = stackalloc byte[2];
                f_bytes(bytes, 2);
                ushort rn = *(ushort*)bytes;
                return rn % (int)length + minValue;
            }
            else
            {
                byte* bytes = stackalloc byte[4];
                f_bytes(bytes, 4);
                uint rn = *(uint*)bytes;
                return (int)(rn % length + minValue);
            }
        }
    }
}
