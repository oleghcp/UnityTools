using System;

namespace UnityUtility.Rng.BytesBased
{
    public class BytesBasedRng : RandomNumberGenerator
    {
        private IRandomBytesProvider _rbp;
#if !UNITY_2021_2_OR_NEWER
        private byte[] _bytes32 = new byte[sizeof(uint)];
        private byte[] _bytes16 = new byte[sizeof(ushort)];
        private byte[] _bytes8 = new byte[sizeof(byte)];
#endif

        public BytesBasedRng(IRandomBytesProvider randomBytesProvider)
        {
            _rbp = randomBytesProvider;
        }

        public override double NextDouble()
        {
            return RngHelper.UintToDouble(RandomUint32());
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
            double normalizedRandomDouble = RngHelper.UintToDouble(RandomUint32());
            return RngHelper.DoubleToFloat(minValue, maxValue, normalizedRandomDouble);
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

        private byte RandomUint8()
        {
#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[sizeof(byte)];
            _rbp.GetBytes(bytes);
            return bytes[0];
#else
            _rbp.GetBytes(_bytes8);
            return _bytes8[0];
#endif
        }

        private ushort RandomUint16()
        {
#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[sizeof(ushort)];
            _rbp.GetBytes(bytes);
            return BitConverter.ToUInt16(bytes);
#else
            _rbp.GetBytes(_bytes16);
            return BitConverter.ToUInt16(_bytes16, 0);
#endif
        }

        private uint RandomUint32()
        {
#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[sizeof(uint)];
            _rbp.GetBytes(bytes);
            return BitConverter.ToUInt32(bytes);
#else
            _rbp.GetBytes(_bytes32);
            return BitConverter.ToUInt32(_bytes32, 0);
#endif
        }
    }
}
