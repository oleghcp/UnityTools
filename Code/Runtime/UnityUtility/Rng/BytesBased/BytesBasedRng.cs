using System;

namespace UnityUtility.Rng.BytesBased
{
    public class BytesBasedRng : RandomNumberGenerator
    {
        private IRandomBytesProvider _rbp;
#if !UNITY_2021_2_OR_NEWER
        private byte[] _bytes64;
        private byte[] _bytes32;
        private byte[] _bytes16;
        private byte[] _bytes8;
#endif

        public BytesBasedRng(IRandomBytesProvider randomBytesProvider)
        {
            _rbp = randomBytesProvider;
#if !UNITY_2021_2_OR_NEWER
            _bytes64 = new byte[sizeof(ulong)];
            _bytes32 = new byte[sizeof(uint)];
            _bytes16 = new byte[sizeof(ushort)];
            _bytes8 = new byte[sizeof(byte)];
#endif
        }

        public override double NextDouble()
        {
            return Sample();
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
            return RngHelper.DoubleToFloat(minValue, maxValue, Sample());
        }

        protected override int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
                return Convert8(length, minValue);

            if (length <= 65536L)
                return Convert16(length, minValue);

            return Convert32(length, minValue);
        }

        private int Convert8(long length, int minValue)
        {
#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[sizeof(byte)];
            _rbp.GetBytes(bytes);
            byte rn = bytes[0];
#else
            _rbp.GetBytes(_bytes8);
            byte rn = _bytes8[0];
#endif
            return rn % (int)length + minValue;
        }

        private int Convert16(long length, int minValue)
        {
#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[sizeof(ushort)];
            _rbp.GetBytes(bytes);
            ushort rn = BitConverter.ToUInt16(bytes);
#else
            _rbp.GetBytes(_bytes16);
            ushort rn = BitConverter.ToUInt16(_bytes16, 0);
#endif
            return rn % (int)length + minValue;
        }

        private int Convert32(long length, int minValue)
        {
#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[sizeof(uint)];
            _rbp.GetBytes(bytes);
            uint rn = BitConverter.ToUInt32(bytes);
#else
            _rbp.GetBytes(_bytes32);
            uint rn = BitConverter.ToUInt32(_bytes32, 0);
#endif
            return (int)(rn % length + minValue);
        }

        private double Sample()
        {
#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[sizeof(ulong)];
            _rbp.GetBytes(bytes);
            ulong rn = BitConverter.ToUInt64(bytes);
#else
            _rbp.GetBytes(_bytes64);
            ulong rn = BitConverter.ToUInt64(_bytes64, 0);
#endif
            return RngHelper.UlongToDouble(rn);
        }
    }
}
