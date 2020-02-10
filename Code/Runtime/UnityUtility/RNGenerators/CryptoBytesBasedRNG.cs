using System;
using System.Security.Cryptography;

namespace UU.RNGenerators
{
    public class CryptoBytesBasedRNG : RNG
    {
        private RNGCryptoServiceProvider m_rng;

        private byte[] m_bytes8;
        private byte[] m_bytes4;
        private byte[] m_bytes2;
        private byte[] m_bytes1;

        public CryptoBytesBasedRNG()
        {
            m_rng = new RNGCryptoServiceProvider();

            m_bytes8 = new byte[sizeof(ulong)];
            m_bytes4 = new byte[sizeof(uint)];
            m_bytes2 = new byte[sizeof(ushort)];
            m_bytes1 = new byte[sizeof(byte)];
        }

        ~CryptoBytesBasedRNG()
        {
            m_rng.Dispose();
        }

        public int Next(int minValue, int maxValue)
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

        public double NextDouble()
        {
            m_rng.GetBytes(m_bytes8);
            f_convert(m_bytes8, out ulong rn);
            rn %= 1000000000000000ul;
            return rn * 0.000000000000001d;
        }

        public byte NextByte()
        {
            m_rng.GetBytes(m_bytes1);
            return m_bytes1[0];
        }

        public void NextBytes(byte[] buffer)
        {
            m_rng.GetBytes(buffer);
        }

        public unsafe void NextBytes(byte* arrayPtr, int length)
        {
            if (arrayPtr == null)
                throw new ArgumentNullException(nameof(arrayPtr), "Pointer cannot be null.");

            for (int i = 0; i < length; i++)
            {
                m_rng.GetBytes(m_bytes1);
                arrayPtr[i] = m_bytes1[0];
            }
        }

        // -- //

        private int f_next(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
            {
                m_rng.GetBytes(m_bytes1);
                byte rn = m_bytes1[0];
                return rn % (int)length + minValue;
            }
            else if (length <= 65536L)
            {
                m_rng.GetBytes(m_bytes2);
                f_convert(m_bytes2, out ushort rn);
                return rn % (int)length + minValue;
            }
            else
            {
                m_rng.GetBytes(m_bytes4);
                f_convert(m_bytes4, out uint rn);
                return (int)(rn % length + minValue);
            }
        }

        private static unsafe void f_convert(byte[] data, out ulong value)
        {
            fixed (byte* ptr = data) { value = *(ulong*)ptr; }
        }

        private static unsafe void f_convert(byte[] data, out uint value)
        {
            fixed (byte* ptr = data) { value = *(uint*)ptr; }
        }

        private static unsafe void f_convert(byte[] data, out ushort value)
        {
            fixed (byte* ptr = data) { value = *(ushort*)ptr; }
        }
    }
}
