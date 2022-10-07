using System;

namespace UnityUtility.Rng.BytesBased
{
    [Serializable]
    internal class Xorshift64Bytes : IRandomBytesProvider
    {
        private readonly int _a;
        private readonly int _b;
        private readonly int _c;

        private byte[] _bytes = new byte[sizeof(ulong)];
        private ulong _num64;
        private int _counter;

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
                if (_counter >= _bytes.Length)
                    UpdateBytes();

                buffer[i] = _bytes[_counter++];
            }
        }

        public void GetBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (_counter >= _bytes.Length)
                    UpdateBytes();

                buffer[i] = _bytes[_counter++];
            }
        }

        private unsafe void UpdateBytes()
        {
            ulong rn = Xorshift64();
            byte* ptr = (byte*)&rn;
            for (int j = 0; j < sizeof(ulong); j++)
            {
                _bytes[j] = ptr[j];
            }
            _counter = 0;
        }

        private ulong Xorshift64()
        {
            ulong x = _num64;
            x ^= x << _a;
            x ^= x >> _b;
            x ^= x << _c;
            return _num64 = x;
        }
    }
}
