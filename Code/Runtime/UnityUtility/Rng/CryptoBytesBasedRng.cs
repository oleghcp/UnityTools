using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
    public class CryptoBytesBasedRng : IRng
    {
        private RNGCryptoServiceProvider m_rng;

        private byte[] m_bytes8;
        private byte[] m_bytes4;
        private byte[] m_bytes2;
        private byte[] m_bytes1;

        public CryptoBytesBasedRng()
        {
            m_rng = new RNGCryptoServiceProvider();

            m_bytes8 = new byte[sizeof(ulong)];
            m_bytes4 = new byte[sizeof(uint)];
            m_bytes2 = new byte[sizeof(ushort)];
            m_bytes1 = new byte[sizeof(byte)];
        }

        ~CryptoBytesBasedRng()
        {
            m_rng.Dispose();
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            return f_next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw Errors.NegativeParameter(nameof(maxValue));

            return f_next(0, maxValue);
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

        public double NextDouble()
        {
            m_rng.GetBytes(m_bytes8);
            ulong rn = BitConverter.ToUInt64(m_bytes8, 0);
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
                throw new ArgumentNullException(nameof(arrayPtr));

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
                ushort rn = BitConverter.ToUInt16(m_bytes2, 0);
                return rn % (int)length + minValue;
            }
            else
            {
                m_rng.GetBytes(m_bytes4);
                uint rn = BitConverter.ToUInt32(m_bytes4, 0);
                return (int)(rn % length + minValue);
            }
        }
    }
}
