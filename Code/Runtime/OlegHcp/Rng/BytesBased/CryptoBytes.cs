using System;
using System.Security.Cryptography;

namespace OlegHcp.Rng.BytesBased
{
    internal class CryptoBytes : IRandomBytesProvider
    {
        private RNGCryptoServiceProvider _rng;

        public CryptoBytes()
        {
            _rng = new RNGCryptoServiceProvider();
        }

        ~CryptoBytes()
        {
            _rng.Dispose();
        }

        public void GetBytes(byte[] buffer)
        {
            _rng.GetBytes(buffer);
        }

        public void GetBytes(Span<byte> buffer)
        {
            _rng.GetBytes(buffer);
        }
    }
}
