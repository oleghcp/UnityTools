using System;
using UnityEngine;

namespace OlegHcp.Rng.BytesBased
{
    public class BytesBasedRng : RandomNumberGenerator
    {
        private IRandomBytesProvider _rbp;

        private byte[] _bytes8 = new byte[sizeof(byte)];
        private byte[] _bytes16 = new byte[sizeof(ushort)];
        private byte[] _bytes32 = new byte[sizeof(uint)];

        public BytesBasedRng(IRandomBytesProvider randomBytesProvider)
        {
            _rbp = randomBytesProvider;
        }

        public override void NextBytes(byte[] buffer)
        {
            _rbp.GetBytes(buffer);
        }

        public override void NextBytes(Span<byte> buffer)
        {
            _rbp.GetBytes(buffer);
        }

        protected override int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
                return RandomUint8() % (int)length + minValue;

            if (length <= 65536L)
                return RandomUint16() % (int)length + minValue;

            return (int)(RandomUint32() % length + minValue);
        }

        protected override float NextInternal(float minValue, float maxValue)
        {
            float normalizedRandomFloat = RandomUint16() / (float)ushort.MaxValue;
            return Mathf.LerpUnclamped(minValue, maxValue, normalizedRandomFloat);
        }

        private byte RandomUint8()
        {
            _rbp.GetBytes(_bytes8);
            return _bytes8[0];
        }

        private ushort RandomUint16()
        {
            _rbp.GetBytes(_bytes16);
            return BitConverter.ToUInt16(_bytes16, 0);
        }

        private uint RandomUint32()
        {
            _rbp.GetBytes(_bytes32);
            return BitConverter.ToUInt32(_bytes32, 0);
        }
    }
}
