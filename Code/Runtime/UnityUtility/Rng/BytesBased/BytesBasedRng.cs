using System;

namespace UnityUtility.Rng.BytesBased
{
    public class BytesBasedRng : RandomNumberGenerator
    {
        private IRandomBytesProvider _rbp;

        private byte[] _bytes64;
        private byte[] _bytes32;
        private byte[] _bytes16;
        private byte[] _bytes8;

        public BytesBasedRng(IRandomBytesProvider randomBytesProvider)
        {
            _rbp = randomBytesProvider;

            _bytes64 = new byte[sizeof(ulong)];
            _bytes32 = new byte[sizeof(uint)];
            _bytes16 = new byte[sizeof(ushort)];
            _bytes8 = new byte[sizeof(byte)];
        }

        public override double NextDouble()
        {
            return NextInternal();
        }

        public override void NextBytes(byte[] buffer)
        {
            _rbp.GetBytes(buffer);
        }

        public override void NextBytes(Span<byte> buffer)
        {
            _rbp.GetBytes(buffer);
        }

        protected override float NextInternal(float minValue, float maxValue)
        {
            return RngHelper.DoubleToFloat(minValue, maxValue, NextInternal());
        }

        protected override int NextInternal(int minValue, int maxValue)
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

        private double NextInternal()
        {
            _rbp.GetBytes(_bytes64);
            ulong rn = BitConverter.ToUInt64(_bytes64, 0);
            return RngHelper.UlongToDouble(rn);
        }
    }
}
