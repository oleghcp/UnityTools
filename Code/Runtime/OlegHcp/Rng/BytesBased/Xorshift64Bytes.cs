using System;

namespace OlegHcp.Rng.BytesBased
{
    [Serializable]
    internal class Xorshift64Bytes : IRandomBytesProvider
    {
        private const int COUNT = sizeof(ulong);

        private readonly int _a;
        private readonly int _b;
        private readonly int _c;

        private byte[] _bytes = new byte[COUNT];
        private int _counter = int.MaxValue;
        private ulong _num64;

        public Xorshift64Bytes() : this(Environment.TickCount)
        {

        }

        public Xorshift64Bytes(int seed) : this(seed, 13, 7, 17)
        {

        }

        public Xorshift64Bytes(int seed, int a, int b, int c)
        {
            if (seed == 0)
                throw new ArgumentException($"Parameter cannot be equal zero.", nameof(seed));

            _num64 = (ulong)seed;
            _a = a;
            _b = b;
            _c = c;
        }

        public void GetBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (_counter >= COUNT)
                    UpdateBytes();

                buffer[i] = _bytes[_counter++];
            }
        }

        public void GetBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (_counter >= COUNT)
                    UpdateBytes();

                buffer[i] = _bytes[_counter++];
            }
        }

        private void UpdateBytes()
        {
            Xorshift64();
            Span<byte> bytes = _bytes;
            bool success = BitConverter.TryWriteBytes(bytes, _num64);

            if (!success)
                throw new InvalidOperationException("Writing Xorshift64Bytes bytes failed.");

            _counter = 0;
        }

        private void Xorshift64()
        {
            ulong x = _num64;
            x ^= x << _a;
            x ^= x >> _b;
            x ^= x << _c;
            _num64 = x;
        }
    }
}
