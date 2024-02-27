using System;

namespace OlegHcp.Rng.BytesBased
{
    public interface IRandomBytesProvider
    {
        void GetBytes(byte[] buffer);
        void GetBytes(Span<byte> buffer);
    }
}
