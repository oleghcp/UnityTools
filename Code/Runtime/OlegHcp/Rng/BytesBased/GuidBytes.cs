using System;

namespace OlegHcp.Rng.BytesBased
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

#if UNITY_2021_2_OR_NEWER
            Span<byte> bytes = stackalloc byte[COUNT + 2];
            if (guid.TryWriteBytes(bytes))
            {
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
            else
            {
                throw new Exception("Cannot write GUID bytes.");
            }
#else
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
#endif
            _counter = 0;
        }
    }
}
