using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
    public class CryptoBytesBasedRng : IRng
    {
        private RNGCryptoServiceProvider _rng;

        private byte[] _bytes8;
        private byte[] _bytes4;
        private byte[] _bytes2;
        private byte[] _bytes1;

        public CryptoBytesBasedRng()
        {
            _rng = new RNGCryptoServiceProvider();

            _bytes8 = new byte[sizeof(ulong)];
            _bytes4 = new byte[sizeof(uint)];
            _bytes2 = new byte[sizeof(ushort)];
            _bytes1 = new byte[sizeof(byte)];
        }

        ~CryptoBytesBasedRng()
        {
            _rng.Dispose();
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

        public double NextDouble()
        {
            _rng.GetBytes(_bytes8);
            ulong rn = BitConverter.ToUInt64(_bytes8, 0);
            rn %= 1000000000000000ul;
            return rn * 0.000000000000001d;
        }

        public byte NextByte()
        {
            _rng.GetBytes(_bytes1);
            return _bytes1[0];
        }

        public void NextBytes(byte[] buffer)
        {
            _rng.GetBytes(buffer);
        }

        public void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                _rng.GetBytes(_bytes1);
                buffer[i] = _bytes1[0];
            }
        }

        // -- //

        private int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
            {
                _rng.GetBytes(_bytes1);
                byte rn = _bytes1[0];
                return rn % (int)length + minValue;
            }
            else if (length <= 65536L)
            {
                _rng.GetBytes(_bytes2);
                ushort rn = BitConverter.ToUInt16(_bytes2, 0);
                return rn % (int)length + minValue;
            }
            else
            {
                _rng.GetBytes(_bytes4);
                uint rn = BitConverter.ToUInt32(_bytes4, 0);
                return (int)(rn % length + minValue);
            }
        }
    }
}
