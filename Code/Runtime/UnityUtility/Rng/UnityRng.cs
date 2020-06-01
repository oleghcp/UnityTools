using System;
using Uerng = UnityEngine.Random;

namespace UnityUtility.Rng
{
    public sealed class UnityRng : IRng
    {
        private Uerng.State m_state;

        public UnityRng() : this(Environment.TickCount)
        {

        }

        public UnityRng(int seed)
        {
            Uerng.InitState(seed);
            m_state = Uerng.state;
        }

        public int Next(int minValue, int maxValue)
        {
            Uerng.state = m_state;
            int value = Uerng.Range(minValue, maxValue);
            m_state = Uerng.state;
            return value;
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValue), nameof(maxValue) + " cannot be negative.");

            return Next(0, maxValue);
        }

        public float NextFloat(float minValue, float maxValue)
        {
            Uerng.state = m_state;
            float value = Uerng.Range(minValue, maxValue);
            m_state = Uerng.state;
            return value;
        }

        public double NextDouble()
        {
            Uerng.state = m_state;
            double value = Uerng.Range(0, int.MaxValue);
            m_state = Uerng.state;
            return value / int.MaxValue;
        }

        public byte NextByte()
        {
            return (byte)Next(0, 256);
        }

        public void NextBytes(byte[] buffer)
        {
            Uerng.state = m_state;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)Uerng.Range(0, 256);
            }
            m_state = Uerng.state;
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr), "Pointer cannot be null.");

            Uerng.state = m_state;
            for (int i = 0; i < length; i++)
            {
                arrayPtr[i] = (byte)Uerng.Range(0, 256);
            }
            m_state = Uerng.state;
        }
    }
}
