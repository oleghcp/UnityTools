using System;
using System.Runtime.CompilerServices;
using UnityUtilityTools;

namespace UnityUtility.Rng.BytesBased
{
    public class BytesBasedRng : IRng
    {
        private IRandomBytesProvider _rbp;

        private byte[] _bytes64;
        private byte[] _bytes32;
        private byte[] _bytes16;
        private byte[] _bytes8;

        public BytesBasedRng(IRandomBytesProvider randomBytesGenerator)
        {
            _rbp = randomBytesGenerator;

            _bytes64 = new byte[sizeof(ulong)];
            _bytes32 = new byte[sizeof(uint)];
            _bytes16 = new byte[sizeof(ushort)];
            _bytes8 = new byte[sizeof(byte)];
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
            _rbp.GetBytes(_bytes64);
            ulong rn = BitConverter.ToUInt64(_bytes64, 0);
            rn %= 1000000000000000ul;
            return rn * 0.000000000000001d;
        }

        public byte NextByte()
        {
            _rbp.GetBytes(_bytes8);
            return _bytes8[0];
        }

        public void NextBytes(byte[] buffer)
        {
            _rbp.GetBytes(buffer);
        }

        public void NextBytes(Span<byte> buffer)
        {
            _rbp.GetBytes(buffer);
        }

        private int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
            {
                _rbp.GetBytes(_bytes8);
                byte rn = _bytes8[0];
                return rn % (int)length + minValue;
            }
            else if (length <= 65536L)
            {
                _rbp.GetBytes(_bytes16);
                ushort rn = BitConverter.ToUInt16(_bytes16, 0);
                return rn % (int)length + minValue;
            }
            else
            {
                _rbp.GetBytes(_bytes32);
                uint rn = BitConverter.ToUInt32(_bytes32, 0);
                return (int)(rn % length + minValue);
            }
        }
    }
}
