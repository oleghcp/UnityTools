using System;

namespace UnityUtility.Rng.BytesBased
{
    internal class GuidBytes : IRandomBytesProvider
    {
        private const int COUNT = 14;
        private const int EXCL_BYTE_INDEX0 = 7;
        private const int EXCL_BYTE_INDEX1 = 8;

        private byte[] _bytes;
        private int _counter = int.MaxValue;

        public GuidBytes()
        {
            if (!BitConverter.IsLittleEndian)
                throw new NotSupportedException("Big endian is not supported.");

            _bytes = new byte[COUNT];
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
            Guid guid = Guid.NewGuid();

            unsafe
            {
                byte* bytes = (byte*)&guid;

                int j = 0;
                for (int i = 0; i < COUNT; i++)
                {
                    while (j == EXCL_BYTE_INDEX0 ||
                           j == EXCL_BYTE_INDEX1)
                    { j++; }

                    _bytes[i] = bytes[j];
                    j++;
                }
            }

            _counter = 0;
        }
    }
}
