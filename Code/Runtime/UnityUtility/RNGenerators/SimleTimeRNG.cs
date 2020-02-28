using System;

namespace UU.RNGenerators
{
    public class SimleTimeRNG : RNG
    {
        private readonly byte MULT;

        private ulong m_seed;
        private ulong m_ticks;

        public SimleTimeRNG() : this(Environment.TickCount) { }

        public SimleTimeRNG(int seed)
        {
            MULT = (byte)((uint)seed % 8 + 2);
            m_seed = (ulong)seed;
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} cannot be more than {nameof(maxValue)}.");

            return (int)f_next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), nameof(maxValue) + " cannot be negative.");

            return (int)f_next(0, maxValue);
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Next(0, 256);
            }
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr), "Pointer cannot be null.");

            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = (byte)Next(0, 256);
            }
        }

        public double NextDouble()
        {
            long rn = f_next(0, 1000000000000000L);
            return rn * 0.000000000000001d;
        }

        public float NextFloat(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} cannot be more than {nameof(maxValue)}.");

            return (float)(NextDouble() * ((double)maxValue - minValue) + minValue);
        }

        public byte NextByte()
        {
            return (byte)Next(0, 256);
        }

        // -- //

        private long f_next(long min, long max)
        {
            ulong size = (ulong)(max - min);
            ulong newTicks = (ulong)DateTime.Now.Ticks;

            if (newTicks > m_ticks)
                m_ticks = newTicks;
            else
                m_ticks++;

            m_seed = MULT * m_seed + m_ticks;

            return (long)(m_seed % size) + min;
        }
    }
}
